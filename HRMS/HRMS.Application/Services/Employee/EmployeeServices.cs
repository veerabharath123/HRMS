using HRMS.Application.Common.Class.LinqExtensions;
using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services.Employee
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponseDto> GetPaginatedEmployeesShortAsync(AdvanceTableRequestDto request)
        {
            var result = await (from emp in _unitOfWork.EmployeeRepo.TableNoTracking
                                join dept in _unitOfWork.DepartmentRepo.TableNoTracking on emp.DepartmentId equals dept.Id
                                join des in _unitOfWork.DesignationRepo.TableNoTracking on emp.DesignationId equals des.Id
                                where !emp.IsDeleted

                                select new EmployeeShortResponseDto
                                {
                                    Id = emp.GuidId,
                                    FullName = emp.FirstName + " " + emp.LastName,
                                    Bio = $"Department: {dept.Name}, Designation: {des.Name}"
                                }
                              )
                              .FilterBy(request.Filter)
                              .SortBy(request.Sort)
                              .PaginateAsync(request.Pagination);

            return ApiResponseDto.SuccessStatus(result);
        }
    }
}
