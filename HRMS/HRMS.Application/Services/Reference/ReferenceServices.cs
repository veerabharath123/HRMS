using HRMS.Application.Common.Interface;
using HRMS.Application.Services.Desgination;
using HRMS.Application.Services.GeneralReference;
using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
