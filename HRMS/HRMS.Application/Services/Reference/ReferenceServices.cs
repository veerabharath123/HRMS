using HRMS.Application.Common.Interface;
using HRMS.Application.Services.Desgination;
using HRMS.Application.Services.GeneralReference;
using HRMS.Domain.Entites;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services
{
    public class ReferenceServices : IReferenceServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentServices _departmentServices;
        private readonly IDesignationServices _designationServices;
        private readonly IGeneralReferenceServices _generalReferenceServices;
        public ReferenceServices(IUnitOfWork unitOfWork, IDepartmentServices departmentServices, 
            IDesignationServices designationServices,
            IGeneralReferenceServices generalReferenceServices)
        {
            _unitOfWork = unitOfWork;
            _departmentServices = departmentServices;
            _designationServices = designationServices;
            _generalReferenceServices = generalReferenceServices;
        }
        public Task<List<BaseRefDto>> GetAllActiveDepartmentsAsync() => _departmentServices.GetAllActiveDepartmentsAsync();
        public Task<List<BaseRefDto>> GetAllActiveDesignationsAsync() => _designationServices.GetAllActiveDesignationsAsync();
        public Task<List<BaseRefDto>> GetAllActiveGendersAsync() => _generalReferenceServices.GetAllActiveGendersAsync();
        public Task<List<BaseRefDto>> GetAllActiveMaritalStatusAsync() => _generalReferenceServices.GetAllActiveMaritalStatusAsync();
        private IQueryable<ModulesResponseDto> GetModulesQuery(Expression<Func<ModuleType,bool>> predicate)
        {
            return _unitOfWork.ModuleTypeRepo.TableNoTracking.Where(predicate)
                .Select(m => new ModulesResponseDto
                {
                    Name = m.Name,
                    Action = m.Action,
                    Controller = m.Controller,
                    IconName = m.IconName,
                    ListOrder = m.ListOrder,
                    Title = m.Module,
                    ParentModuleId = m.ParentModuleId,
                    RouteValues = m.HasChildren 
                                    ? new Dictionary<string, object> { { "module", m.Id } }
                                    : null
                });
        }
        public async Task<List<ModulesResponseDto>> GetAllActiveModulesAsync()
        {
            return await GetModulesQuery(x => !x.IsDeleted && x.ParentModuleId == null)
                .OrderBy(m => m.ListOrder)
                .ToListAsync();
        }
        public async Task<List<ModulesResponseDto>> GetAllActiveModulesByParentIdAsync(int parentId)
        {
            return await GetModulesQuery(x => !x.IsDeleted && x.ParentModuleId == parentId)
                .OrderBy(m => m.ListOrder)
                .ToListAsync();
        }
    }
}
