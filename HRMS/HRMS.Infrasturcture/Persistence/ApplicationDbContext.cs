using HRMS.Application.Common.Interface;
using HRMS.Domain.Common;
using HRMS.Domain.Entites;
using HRMS.Infrastructure.Persistence.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Persistence
{
    public class ApplicationDbContext : BaseDbContext, IApplicationDbContext
    {
        private readonly DateTime _currentDateTime;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _currentDateTime = DateTime.Now;
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> User { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Permissions> Permissions { get; set; }

        public Task<int> SaveChangesAsync()
        {
            int usernr = GetCurrentUser();

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (usernr != -1)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            SetAuditFieldsCreated(entry, usernr);
                            break;
                        case EntityState.Modified:
                            SetAuditFieldsModified(entry, usernr);
                            break;
                    }
                }
            }
            return base.SaveChangesAsync();
        }
        public int GetCurrentUser()
        {
            return 1;
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || user.Identity is null || !user.Identity.IsAuthenticated)
                return 0;

            var userIdClaim = user.FindFirst("UserId");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return 0;
        }

        private void SetAuditFieldsCreated(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IAuditableEntity> entry1, int usernr)
        {
            entry1.Entity.CreatedUser = usernr;
            entry1.Entity.CreatedDate = _currentDateTime.Date;
            entry1.Entity.CreatedTime = TimeSpan.Parse(_currentDateTime.ToString("HH:mm:ss"));
            SetAuditFieldsModified(entry1, usernr);


        }
        private void SetAuditFieldsModified(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IAuditableEntity> entry1, int usernr)
        {
            entry1.Entity.UpdatedTime = TimeSpan.Parse(_currentDateTime.ToString("HH:mm:ss"));
            entry1.Entity.UpdatedUser = usernr;
            entry1.Entity.UpdatedDate = _currentDateTime.Date;
        }


        public override DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            ApplyEntityConfigurationMaster(modelBuilder, GetType());
        }
    }
}
