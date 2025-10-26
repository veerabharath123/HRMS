using HRMS.Application.Common.Interface;
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
        public ReferenceServices(IUnitOfWork unitOfWork, IDepartmentServices departmentServices)
        {
            _unitOfWork = unitOfWork;
            _departmentServices = departmentServices;
        }
        public Task<List<BaseRefDto>> GetAllActiveDepartmentsAsync() => _departmentServices.GetAllActiveDepartmentsAsync();
    }
}
