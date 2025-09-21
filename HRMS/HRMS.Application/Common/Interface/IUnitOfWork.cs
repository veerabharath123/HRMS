using HRMS.Domain.Entites;

namespace HRMS.Application.Common.Interface
{
    public interface IUnitOfWork
    {
        IRepository<User> UserRepo { get; }
        IRepository<Roles> RolesRepo { get; }
        IRepository<Permissions> PermissionsRepo { get; }
        IRepository<RolePermissions> RolePermissionsRepo { get; }
        IRepository<UserRoles> UserRolesRepo { get; }
        IRepository<StoredFiles> StoredFilesRepo { get; }
        IRepository<FileLocationConfigurations> FileLocationConfigurationsRepo { get; }
        IRepository<SystemSettings> SystemSettingsRepo { get; }


        Task<bool> SaveAsync();
        Task<int> SaveChangesAsync();
        int Save();
        void BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();

    }
}
