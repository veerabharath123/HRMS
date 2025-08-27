using HRMS.Application.Common.Interface;
using HRMS.Domain.Entites;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRMS.Infrasturcture.Persistence.Configuration
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        IDbContextTransaction dbContextTransaction;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        #region private repositories

        private IRepository<User> _userRepo;
        private IRepository<Roles> _rolesRepo;
        private IRepository<Permissions> _permissionsRepo;
        private IRepository<RolePermissions> _rolePermissionsRepo;
        private IRepository<UserRoles> _userRolesRepo;

        #endregion private repositories

        #region public repositories
        public IRepository<User> UserRepo
        {
            get
            {
                _userRepo ??= new EFRepository<User>(_context);
                return _userRepo;
            }
        }

        public IRepository<Roles> RolesRepo
        {
            get
            {
                _rolesRepo ??= new EFRepository<Roles>(_context);
                return _rolesRepo;
            }
        }
        public IRepository<Permissions> PermissionsRepo
        {
            get
            {
                _permissionsRepo ??= new EFRepository<Permissions>(_context);
                return _permissionsRepo;
            }
        }
        public IRepository<RolePermissions> RolePermissionsRepo
        {
            get
            {
                _rolePermissionsRepo ??= new EFRepository<RolePermissions>(_context);
                return _rolePermissionsRepo;
            }
        }
        public IRepository<UserRoles> UserRolesRepo
        {
            get
            {
                _userRolesRepo ??= new EFRepository<UserRoles>(_context);
                return _userRolesRepo;
            }
        }
        #endregion public repositories

        #region transaction methods
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public int Save()
        {
            return _context.SaveChanges();
        }
        public void BeginTransaction()
        {
            dbContextTransaction = _context.Database.BeginTransaction();
        }
        public void CommitTransaction()
        {
            dbContextTransaction?.Commit();
        }
        public void RollBackTransaction()
        {
            dbContextTransaction?.Rollback();
        }

        #endregion transaction methods

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
