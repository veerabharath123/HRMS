using HRMS.SharedKernel.Models.Common.Enum;
using HRMS.SharedKernel.Models.Request;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

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

                Expression expr = filter.Comparator switch 
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
                    combined = filter.Operator == FilterOperator.And
                        ? Expression.AndAlso(combined, expr)
                        : Expression.OrElse(combined, expr);
            }

            return combined == null
                ? source
                : source.Where(Expression.Lambda<Func<T, bool>>(combined, param));
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
    }
}
