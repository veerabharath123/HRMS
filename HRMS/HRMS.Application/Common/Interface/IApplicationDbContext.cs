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

    }
}
