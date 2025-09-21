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

        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<StoredFiles> StoredFiles { get; set; }
        public DbSet<FileLocationConfigurations> FileLocationConfigurations { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }

        public Task<int> SaveChangesAsync()
        {
            string user = GetCurrentUser();

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (string.IsNullOrWhiteSpace(user))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            SetAuditFieldsCreated(entry, user);
                            break;
                        case EntityState.Modified:
                            SetAuditFieldsModified(entry, user);
                            break;
                    }
                }
            }
            return base.SaveChangesAsync();
        }
        public string GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || user.Identity is null || !user.Identity.IsAuthenticated)
                return string.Empty;

            var username = user.FindFirst("Username");

            return username?.Value ?? string.Empty;
        }

        private void SetAuditFieldsCreated(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IAuditableEntity> entry1, string user)
        {
            entry1.Entity.CreatedUser = user;
            entry1.Entity.CreatedDate = _currentDateTime.Date;
            SetAuditFieldsModified(entry1, user);


        }
        private void SetAuditFieldsModified(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IAuditableEntity> entry1, string user)
        {
            entry1.Entity.UpdatedUser = user;
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
