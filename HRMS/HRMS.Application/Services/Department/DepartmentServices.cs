using HRMS.Application.Common.Interface;
using HRMS.Domain.Constants;
using HRMS.Domain.Entites;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services
{
    public class DepartmentServices : IDepartmentServices
    {
        private IUnitOfWork _unitOfWork;
        private ICacheService _cache;
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
