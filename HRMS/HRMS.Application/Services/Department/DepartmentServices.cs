using HRMS.Application.Common.Interface;
using HRMS.Domain.Constants;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Services
{
    public class DepartmentServices : IDepartmentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        public DepartmentServices(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<List<BaseRefDto>> GetAllActiveDepartmentsAsync()
        {
            var results = await _cache.GetOrCreateAsync(
                GeneralConstants.CachedModules.DepartmentManagement,
                () => 
                    _unitOfWork.DepartmentRepo.TableNoTracking
                    .Where(d => d.IsActive && !d.IsDeleted)
                    .Select(d => new BaseRefDto
                    {
                        Id = d.GuidId,
                        Name = d.Name
                    }).ToListAsync()
            );

            return results;
        }
    }
}
