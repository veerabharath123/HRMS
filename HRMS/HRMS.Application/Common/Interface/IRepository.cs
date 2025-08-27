using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Delete(object id);
        Task<bool> Delete(Expression<Func<T, bool>> predicate);
        Task<T> Get(object id);
        Task<T> Get(Expression<Func<T, bool>> predicate);
        Task<int?> GetIdByGuid(Guid guid);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        Task<int> Count(Expression<Func<T, bool>> predicate);
        Task<int> Count();
        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
    }
}
