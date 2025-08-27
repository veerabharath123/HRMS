using HRMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Common.Interface
{
    public interface IApplicationDbContext
    {
        DbSet<User> User { get; set; }
        DbSet<Roles> Roles { get; set; }
        DbSet<UserRoles> UserRoles { get; set; }
        DbSet<RolePermissions> RolePermissions { get; set; }
        DbSet<Permissions> Permissions { get; set; }

    }
}
