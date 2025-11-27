using HRMS.Application.Common.Interface;
using HRMS.Domain.Entites;
using HRMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace HRMS.Infrastructure.Persistence.Configuration
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        IDbContextTransaction dbContextTransaction;
        private readonly IMemoryCache _cache;
        public UnitOfWork(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        #region private repositories

        private IRepository<User> _userRepo;
        private IRepository<Roles> _rolesRepo;
        private IRepository<Permissions> _permissionsRepo;
        private IRepository<RolePermissions> _rolePermissionsRepo;
        private IRepository<UserRoles> _userRolesRepo;
        private IRepository<StoredFiles> _storedFilesRepo;
        private IRepository<FileLocationConfigurations> _fileLocationConfigurationsRepo;
        private IRepository<SystemSettings> _systemSettingsRepo;
        private IRepository<EmployeeGuardian> _employeeGuardianRepo;
        private IRepository<EmployeeContact> _employeeContactRepo;
        private IRepository<Employee> _employeeRepo;
        private IRepository<Designation> _designationRepo;
        private IRepository<Department> _departmentRepo;
        private IRepository<GeneralReference> _generalReferenceRepo;

        // chat related
        private IRepository<ConversationType> _conversationTypesRepo;
        private IRepository<Conversation> _conversationsRepo;
        private IRepository<Message> _messagesRepo;
        private IRepository<MessageStatus> _messageStatusRepo;
        private IRepository<Attachment> _attachmentsRepo;
        private IRepository<ConversationMember> _conversationMembersRepo;

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
        public IRepository<StoredFiles> StoredFilesRepo
        {
            get
            {
                _storedFilesRepo ??= new EFRepository<StoredFiles>(_context);
                return _storedFilesRepo;
            }
        }
        public IRepository<FileLocationConfigurations> FileLocationConfigurationsRepo
        {
            get
            {
                _fileLocationConfigurationsRepo ??= new EFRepository<FileLocationConfigurations>(_context);
                return _fileLocationConfigurationsRepo;
            }
        }
        public IRepository<SystemSettings> SystemSettingsRepo
        {
            get
            {
                _systemSettingsRepo ??= new EFRepository<SystemSettings>(_context);
                return _systemSettingsRepo;
            }
        }
        public IRepository<EmployeeGuardian> EmployeeGuardianRepo
        {
            get
            {
                _employeeGuardianRepo ??= new EFRepository<EmployeeGuardian>(_context);
                return _employeeGuardianRepo;
            }
        }
        public IRepository<EmployeeContact> EmployeeContactRepo
        {
            get
            {
                _employeeContactRepo ??= new EFRepository<EmployeeContact>(_context);
                return _employeeContactRepo;
            }
        }
        public IRepository<Employee> EmployeeRepo
        {
            get
            {
                _employeeRepo ??= new EFRepository<Employee>(_context);
                return _employeeRepo;
            }
        }
        public IRepository<Designation> DesignationRepo
        {
            get
            {
                _designationRepo ??= new EFRepository<Designation>(_context);
                return _designationRepo;
            }
        }
        public IRepository<Department> DepartmentRepo
        {
            get
            {
                _departmentRepo ??= new EFRepository<Department>(_context);
                return _departmentRepo;
            }
        }
        public IRepository<GeneralReference> GeneralReferenceRepo
        {
            get
            {
                _generalReferenceRepo ??= new EFRepository<GeneralReference>(_context);
                return _generalReferenceRepo;
            }
        }

        // chat related
        public IRepository<ConversationType> ConversationTypesRepo
        {
            get
            {
                _conversationTypesRepo ??= new EFRepository<ConversationType>(_context);
                return _conversationTypesRepo;
            }
        }

        public IRepository<Conversation> ConversationsRepo
        {
            get
            {
                _conversationsRepo ??= new EFRepository<Conversation>(_context);
                return _conversationsRepo;
            }
        }

        public IRepository<Message> MessagesRepo
        {
            get
            {
                _messagesRepo ??= new EFRepository<Message>(_context);
                return _messagesRepo;
            }
        }

        public IRepository<MessageStatus> MessageStatusRepo
        {
            get
            {
                _messageStatusRepo ??= new EFRepository<MessageStatus>(_context);
                return _messageStatusRepo;
            }
        }

        public IRepository<Attachment> AttachmentsRepo
        {
            get
            {
                _attachmentsRepo ??= new EFRepository<Attachment>(_context);
                return _attachmentsRepo;
            }
        }

        public IRepository<ConversationMember> ConversationMembersRepo
        {
            get
            {
                _conversationMembersRepo ??= new EFRepository<ConversationMember>(_context);
                return _conversationMembersRepo;
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
