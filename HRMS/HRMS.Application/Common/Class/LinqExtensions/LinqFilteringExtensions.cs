using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Common.Enum;
using HRMS.SharedKernel.Models.Request;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace HRMS.Application.Common.Class.LinqExtensions
{
    public static class FilteringExtensions
    {
        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> _propCache = new();

        public static IQueryable<T> FilterBy<T>(
            this IQueryable<T> source, FilterRequestDto? request)
        {
            if (request?.Filters is null || request.Filters.Count == 0)
                return source;

            var type = typeof(T);
            var param = Expression.Parameter(type, "x");
            Expression? combined = null;

            foreach (var filter in request.Filters)
            {
                var prop = _propCache.GetOrAdd(
                    (type, filter.PropertyName),
                    k => k.Item1.GetProperty(
                            k.Item2,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)) 
                    ?? throw new ArgumentException($"Invalid property '{filter.PropertyName}' on {type.Name}");

                Expression member = Expression.Property(param, prop);
                // Ensure proper type conversion (handles nullables too)
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var constant = Expression.Constant(Convert.ChangeType(filter.Value, targetType), targetType);
                if(prop.PropertyType != targetType) member = Expression.Convert(member, targetType);

                Expression expr = (Enum.TryParse(filter.Comparator, out FilterComparator result) ? result : FilterComparator.NotImp) switch 
                { 
                    FilterComparator.Equals => Expression.Equal(member, constant), 
                    FilterComparator.NotEquals => Expression.NotEqual(member, constant), 
                    FilterComparator.LessThan => Expression.LessThan(member, constant), 
                    FilterComparator.GreaterThan => Expression.GreaterThan(member, constant), 
                    FilterComparator.LessThanOrEqual => Expression.LessThanOrEqual(member, constant), 
                    FilterComparator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant), 
                    FilterComparator.Contains => EnsureStringCall(member, "Contains", constant, prop), 
                    FilterComparator.StartsWith => EnsureStringCall(member, "StartsWith", constant, prop), 
                    FilterComparator.EndsWith => EnsureStringCall(member, "EndsWith", constant, prop), 
                    _ => throw new NotSupportedException($"Comparator {filter.Comparator} not supported") 
                };

                if (combined == null)
                    combined = expr;
                else
                    combined = (Enum.TryParse(filter.Operator, out FilterOperator op) ? op : FilterOperator.And) == FilterOperator.And
                        ? Expression.AndAlso(combined, expr)
                        : Expression.OrElse(combined, expr);
            }

            return combined == null
                ? source
                : source.Where(Expression.Lambda<Func<T, bool>>(combined, param));
        }
        public static IQueryable<T> FilterBy<T>(this IQueryable<T> source, FilterGroupDto? group)
        {
            if (group == null) return source;

            var param = Expression.Parameter(typeof(T), "x");

            var expr = BuildGroupExpression<T>(group, param);

            if (expr == null)
                return source;

            return source.Where(Expression.Lambda<Func<T, bool>>(expr, param));
        }


        private static MethodCallExpression EnsureStringCall(Expression member, string method, ConstantExpression constant, PropertyInfo prop)
        {
            if (prop.PropertyType != typeof(string))
                throw new InvalidOperationException($"Comparator '{method}' is only valid for string properties.");

            return Expression.Call(
                Expression.Call(member, nameof(string.ToLower), Type.EmptyTypes),
                typeof(string).GetMethod(method, [typeof(string)])!,
                Expression.Call(constant, nameof(string.ToLower), Type.EmptyTypes));
        }
        private static Expression? BuildGroupExpression<T>(FilterGroupDto group, ParameterExpression param)
        {
            Expression? combined = null;
            var type = typeof(T);

            // Build filter expressions
            if (group.Filters != null)
            {
                foreach (var filter in group.Filters)
                {
                    var expr = BuildFilterExpression<T>(filter, param, type);
                    combined = CombineExpressions(combined, expr, group.Operator);
                }
            }

            // Build nested groups
            if (group.Groups != null)
            {
                foreach (var childGroup in group.Groups)
                {
                    var childExpr = BuildGroupExpression<T>(childGroup, param);
                    combined = CombineExpressions(combined, childExpr, group.Operator);
                }
            }

            return combined;
        }


        // Combines two expressions using the specified operator (And/Or).
        private static Expression? CombineExpressions(Expression? left, Expression? right, string op)
        {
            if (right == null) return left;
            if (left == null) return right;

            var opEnum = Enum.TryParse<FilterOperator>(op, true, out var parsedOp) ? parsedOp : FilterOperator.And;

            return opEnum == FilterOperator.Or
                ? Expression.OrElse(left, right)
                : Expression.AndAlso(left, right);
        }

        // Build a single filter expression for an item (e.g. x => x.Department == "HR")
        private static Expression? BuildFilterExpression<T>(FilterOptionDto filter, ParameterExpression param, Type type)
        {
            if (string.IsNullOrWhiteSpace(filter.PropertyName))
                return null;

            var prop = _propCache.GetOrAdd(
                (type, filter.PropertyName),
                k => k.Item1.GetProperty(k.Item2, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
            );

            if (prop == null)
                throw new ArgumentException($"Invalid property '{filter.PropertyName}' on {type.Name}");

            Expression member = Expression.Property(param, prop);

            // Determine target type and convert filter.Value to that type
            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            object? rawValue = filter.Value;
            object? convertedValue = null;

            if (rawValue == null)
            {
                convertedValue = null;
            }
            else
            {
                // If incoming is string and target is DateTime or numeric, try to convert
                try
                {
                    if (rawValue is string s && targetType != typeof(string))
                    {
                        if (targetType == typeof(DateTime))
                            convertedValue = DateTime.Parse(s);
                        else if (targetType.IsEnum)
                            convertedValue = Enum.Parse(targetType, s, true);
                        else
                            convertedValue = Convert.ChangeType(s, targetType);
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(rawValue, targetType);
                    }
                }
                catch
                {
                    // fallback: try ChangeType directly (may throw)
                    convertedValue = Convert.ChangeType(rawValue, targetType);
                }
            }

            Expression? constant = convertedValue == null
                ? Expression.Constant(null, prop.PropertyType)
                : Expression.Constant(convertedValue, targetType);

            // If member is nullable and targetType is non-nullable, convert member to targetType
            if (prop.PropertyType != targetType)
                member = Expression.Convert(member, targetType);

            // Parse comparator enum
            var comparator = Enum.TryParse<FilterComparator>(filter.Comparator, true, out var comp) ? comp : FilterComparator.NotImp;

            return comp switch
            {
                FilterComparator.Equals => convertedValue == null
                    ? Expression.Equal(member, Expression.Constant(null, member.Type))
                    : Expression.Equal(member, constant),

                FilterComparator.NotEquals => convertedValue == null
                    ? Expression.NotEqual(member, Expression.Constant(null, member.Type))
                    : Expression.NotEqual(member, constant),

                FilterComparator.LessThan => Expression.LessThan(member, constant),
                FilterComparator.GreaterThan => Expression.GreaterThan(member, constant),
                FilterComparator.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
                FilterComparator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),

                FilterComparator.Contains => EnsureStringCall(member, "Contains", constant, prop),
                FilterComparator.StartsWith => EnsureStringCall(member, "StartsWith", constant, prop),
                FilterComparator.EndsWith => EnsureStringCall(member, "EndsWith", constant, prop),

                _ => throw new NotSupportedException($"Comparator '{filter.Comparator}' not supported")
            };
        }

        private static MethodCallExpression EnsureStringCall(Expression member, string method, Expression constant, PropertyInfo prop)
        {
            // If the property is not a string, attempt to convert to string (useful for numeric codes),
            // but prefer to require string type for clarity. Here I'll allow string-only to keep EF translation safe.
            if (prop.PropertyType != typeof(string) && Nullable.GetUnderlyingType(prop.PropertyType) != typeof(string))
                throw new InvalidOperationException($"Comparator '{method}' is only valid for string properties. Property '{prop.Name}' type: {prop.PropertyType.Name}");

            // member.ToLower().Method(constant.ToLower())
            var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
            var callMemberToLower = Expression.Call(member, toLower);

            // constant might be an Expression.Constant of targetType (string)
            Expression constAsString = constant;
            if (constant is ConstantExpression ce && ce.Value != null && ce.Type != typeof(string))
            {
                // convert constant to string
                constAsString = Expression.Constant(ce.Value.ToString(), typeof(string));
            }

            var callConstToLower = Expression.Call(constAsString, toLower);

            var methodInfo = typeof(string).GetMethod(method, new[] { typeof(string) })
                             ?? throw new InvalidOperationException($"string.{method} not found");

            return Expression.Call(callMemberToLower, methodInfo, callConstToLower);
        }


    }
}
