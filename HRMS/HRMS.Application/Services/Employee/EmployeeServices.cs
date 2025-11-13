using HRMS.Application.Common.Class.LinqExtensions;
using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Records.EmployeeRecords;

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
        public async Task<ApiResponseDto> AddEmployeeAsync()
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Perform multiple operations here

                _unitOfWork.CommitTransaction();

                return ApiResponseDto.SuccessStatus(null);
            }
            catch (Exception)
            {
                _unitOfWork.RollBackTransaction();
                throw;
            }
        }
        public async Task<ApiResponseDto> InsertEmployeeAsync(EmployeeDto request)
        {
            var newEmployee = new Domain.Entites.Employee();

            newEmployee.Add(new EmployeeFullRecord(
                request.LastName,
                request.FirstName,
                request.BirthDate,
                request.GenderId,
                request.MaritalStatusId,
                request.JoiningDate,
                request.DepartmentId,
                request.DesignationId,
                request.RelievingDate,
                request.ReportingManagerId
            ));

            var profilePictureId = await _unitOfWork.StoredFilesRepo.GetIdByGuid(request.PhotoPictureId);
            newEmployee.AddProfilePicture(profilePictureId);

            _unitOfWork.EmployeeRepo.Add(newEmployee);
            var saved = await _unitOfWork.SaveAsync();

            return ApiResponseDto.FlagStatus(saved, newEmployee.Id);
        }

    }
}
