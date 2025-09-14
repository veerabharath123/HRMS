using HRMS.SharedKernel.Models.Request;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace HRMS.Application.Common.Class.LinqExtensions
{
    public static class LinqSortingExtensions
    {
        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> _cache = new();

        public static IQueryable<T> SortBy<T>(
            this IQueryable<T> source, SortRequestDto? request)
        {
            if (request?.SortOptions is null || request.SortOptions.Count == 0)
                return source;

            var type = typeof(T);
            IOrderedQueryable<T>? query = null;

            foreach (var (opt, idx) in request.SortOptions.Select((o, i) => (o, i)))
            {
                var prop = _cache.GetOrAdd((type, opt.PropertyName),
                              k => k.Item1.GetProperty(k.Item2,
                                  BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance))
                          ?? throw new ArgumentException(
                                $"Invalid sort property '{opt.PropertyName}' on {type.Name}");

                var param = Expression.Parameter(type, "x");
                var lambda = Expression.Lambda(Expression.Property(param, prop), param);

                var method = (idx == 0, opt.Descending) switch
                {
                    (true, false) => "OrderBy",
                    (true, true) => "OrderByDescending",
                    (false, false) => "ThenBy",
                    _ => "ThenByDescending"
                };

                query = (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(
                    Expression.Call(typeof(Queryable), method,
                        [type, prop.PropertyType],
                        (query ?? source).Expression, Expression.Quote(lambda)));
            }

            return query ?? source;
        }
    }
}
