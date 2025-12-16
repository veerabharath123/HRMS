using HRMS.Application.Common.Interface;
using HRMS.Domain.Constants;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Constants.GeneralConstants;

namespace HRMS.Application.Services.GeneralReference
{
    public class GeneralReferenceServices : IGeneralReferenceServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;
        public GeneralReferenceServices(IUnitOfWork unitOfWork, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }
        private IQueryable<BaseRefDto> GetAllActiveReferenceByCategoryQuery(GeneralReferenceCategories category)
        {
            return _unitOfWork.GeneralReferenceRepo.TableNoTracking
                    .Where(d => d.IsActive && !d.IsDeleted && d.Category == category.ToString())
                    .Select(d => new BaseRefDto
                    {
                        Id = d.Id,
                        Name = d.Value
                    });
        }
        public Task<List<BaseRefDto>> GetAllActiveGendersAsync()
        {
            return _cache.GetOrCreateAsync(
                CachedModules.GenderManagement,
                () => GetAllActiveReferenceByCategoryQuery(GeneralReferenceCategories.Gender).ToListAsync()
            );
        }
        public Task<List<BaseRefDto>> GetAllActiveMaritalStatusAsync()
        {
            return _cache.GetOrCreateAsync(
                CachedModules.MaritalStatusManagement,
                () => GetAllActiveReferenceByCategoryQuery(GeneralReferenceCategories.MaritalStatus).ToListAsync()
            );
        }
    }
}
