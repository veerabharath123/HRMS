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
        IRepository<EmployeeGuardian> EmployeeGuardianRepo { get; }
        IRepository<EmployeeContact> EmployeeContactRepo { get; }
        IRepository<Employee> EmployeeRepo { get; }
        IRepository<Designation> DesignationRepo { get; }
        IRepository<Department> DepartmentRepo { get; }
        IRepository<GeneralReference> GeneralReferenceRepo { get; }
        IRepository<ModuleType> ModuleTypeRepo { get; }
        IRepository<ConversationType> ConversationTypesRepo { get; }
        IRepository<Conversation> ConversationsRepo { get; }
        IRepository<ConversationParticipants> ConversationParticipantsRepo { get; }
        IRepository<Message> MessagesRepo { get; }
        IRepository<MessageStatus> MessageStatusRepo { get; }
        IRepository<Attachment> AttachmentsRepo { get; }
        IRepository<ConversationMember> ConversationMembersRepo { get; }




        Task<bool> SaveAsync();
        Task<int> SaveChangesAsync();
        int Save();
        void BeginTransaction();
        void CommitTransaction();
        void RollBackTransaction();

    }
}
