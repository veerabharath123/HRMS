using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HRMS.Application.Common.Interface;
using HRMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Persistence.Configuration
{
    internal class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<TEntity> _entities;

        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        protected string GetFullErrorTextAndRollBackEntityChanges(DbUpdateException exception)
        {
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();
                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch (InvalidOperationException)
                    {

                    }
                });
            }

            try
            {
                _context.SaveChanges();
                return exception.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                _entities ??= _context.Set<TEntity>();
                return _entities;
            }
        }


        public IQueryable<TEntity> Table => Entities;

        public IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        public virtual void Add(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await Entities.CountAsync(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<int> Count()
        {
            try
            {
                return await Entities.CountAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<bool> Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                    return false;

                Entities.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<bool> Delete(object id)
        {
            var entity = await Entities.FindAsync(id);
            return await Delete(entity);
        }

        public virtual async Task<bool> Delete(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var entities = Entities.Where(predicate);
                Entities.RemoveRange(entities);
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception(GetFullErrorTextAndRollBackEntityChanges(ex), ex);
            }
        }

        public virtual async Task<TEntity> Get(object id)
        {
            try
            {
                return await Entities.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await Entities.FirstOrDefaultAsync(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public virtual async Task<int?> GetIdByGuid(Guid guid)
        {
            try
            {
                int? id = await Entities
                    .Where(e => EF.Property<Guid>(e, "GuidId") == guid)
                    .Select(e => EF.Property<int>(e, "Id"))
                    .FirstOrDefaultAsync();

                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            try
            {
                return Entities;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return Entities.Where(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Update(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }
        public virtual void SaveChangesAsync()
        {
            _context.SaveChangesAsync();
        }

    }
}
