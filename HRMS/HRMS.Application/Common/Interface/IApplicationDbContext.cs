using HRMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Common.Interface
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Roles> Roles { get; set; }
        DbSet<UserRoles> UserRoles { get; set; }
        DbSet<RolePermissions> RolePermissions { get; set; }
        DbSet<Permissions> Permissions { get; set; }
        DbSet<StoredFiles> StoredFiles { get; set; }
        DbSet<FileLocationConfigurations> FileLocationConfigurations { get; set; }
        DbSet<SystemSettings> SystemSettings { get; set; }
        DbSet<EmployeeGuardian> EmployeeGuardian { get; set; }
        DbSet<EmployeeContact> EmployeeContact { get; set; }
        DbSet<Employee> Employee { get; set; }
        DbSet<Designation> Designation { get; set; }
        DbSet<Department> Department { get; set; }
        DbSet<GeneralReference> GeneralReference { get; set; }
        DbSet<ConversationType> ConversationTypes { get; set; }
        DbSet<Conversation> Conversations { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<MessageStatus> MessageStatus { get; set; }
        DbSet<Attachment> Attachments { get; set; }
        DbSet<ConversationMember> ConversationMembers { get; set; }
    }
}
