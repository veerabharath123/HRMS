using HRMS.Application.Common.Interface;
using HRMS.Domain.Constants;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services.Desgination
{
    public class DesignationServices : IDesignationServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        public DesignationServices(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }
        public async Task<List<BaseRefDto>> GetAllActiveDesignationsAsync()
        {
            var results = await _cache.GetOrCreateAsync(
                GeneralConstants.CachedModules.DesignationManagement,
                () =>
                    _unitOfWork.DesignationRepo.TableNoTracking
                    .Where(d => d.IsActive && !d.IsDeleted)
                    .Select(d => new BaseRefDto
                    {
                        Id = d.Id,
                        Name = d.Name
                    }).ToListAsync()
            );

            return results;
        }
    }
}
