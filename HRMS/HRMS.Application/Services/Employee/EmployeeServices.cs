using AutoMapper;
using HRMS.Application.Common.Class.LinqExtensions;
using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Records.EmployeeRecords;

namespace HRMS.Application.Services.Employee
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmployeeServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        public async Task<ApiResponseDto> AddEmployeeAsync(InsertEmployeeRequestDto request)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var newEmployee = await InsertEmployeeAsync(request);

                _unitOfWork.CommitTransaction();

                return newEmployee;
            }
            catch (Exception)
            {
                _unitOfWork.RollBackTransaction();
                throw;
            }
        }
        private Task<bool> EmployeeExistAsync(Expression<Func<Domain.Entites.Employee, bool>> predicate)
        {
            return _unitOfWork.EmployeeRepo.TableNoTracking.AnyAsync(predicate);
        }
        public async Task<ApiResponseDto> InsertEmployeeAsync(InsertEmployeeRequestDto request)
        {
            var exists = await EmployeeExistAsync(e => 
                e.LastName == request.Employee.LastName 
                && e.FirstName == request.Employee.FirstName
                && !e.IsDeleted);

            if (exists) return ApiResponseDto.FailureStatus("Employee with the same name already exists.");

            var newEmployee = new Domain.Entites.Employee();

            newEmployee.Add(_mapper.Map<EmployeeFullRecord>(request.Employee));

            var profilePictureId = await _unitOfWork.StoredFilesRepo.GetIdByGuid(request.Employee.PhotoPictureId);
            newEmployee.AddProfilePicture(profilePictureId);

            _unitOfWork.EmployeeRepo.Add(newEmployee);
            var saved = await _unitOfWork.SaveAsync();

            return ApiResponseDto.FlagStatus(saved, newEmployee.Id);
        }

    }
}
