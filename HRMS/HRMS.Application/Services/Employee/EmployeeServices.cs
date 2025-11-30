using AutoMapper;
using HRMS.Application.Common.Class.LinqExtensions;
using HRMS.Application.Common.Interface;
using HRMS.Application.Services.File;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Records.EmployeeRecords;

namespace HRMS.Application.Services.Employee
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmployeeServices(IUnitOfWork unitOfWork, IMapper mapper, IFileServices fileServices, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileServices = fileServices;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponseDto> GetPaginatedEmployeesShortAsync(AdvanceTableRequestDto request)
        {
            var result = await (from emp in _unitOfWork.EmployeeRepo.TableNoTracking
                                join dept in _unitOfWork.DepartmentRepo.TableNoTracking on emp.DepartmentId equals dept.Id
                                join des in _unitOfWork.DesignationRepo.TableNoTracking on emp.DesignationId equals des.Id
                                where !emp.IsDeleted

                                select new EmployeeShortResponseDto
                                {
                                    Id = emp.Id,
                                    Bio = $"Department: {dept.Name}, Designation: {des.Name}",
                                    Department = dept.Name,
                                    Designation = des.Name,
                                    LastName = emp.LastName,
                                    FirstName = emp.FirstName
                                }
                              )
                              .FilterBy(request.FilterGroup)
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

        public async Task<ApiResponseDto> GetEmployeeImagesAsync(ListIdRequestDto request)
        {
            var employee = await _unitOfWork.EmployeeRepo.Table
                .Where(e => request.IdList.Contains(e.Id) && !e.IsDeleted).ToListAsync();

            var employeeImages = new List<EmpImageResponseDto>();

            foreach (var emp in employee)
            {
                if(emp.PhotoPictureId is null) 
                    continue;

                var result = await _fileServices.GetFileByStoredFileIdAsync(emp.PhotoPictureId.Value);

                employeeImages.Add(new EmpImageResponseDto
                {
                    Id = emp.Id,
                    ImageBase64 = result
                });
            }

            return ApiResponseDto.SuccessStatus(employeeImages);
        }
        private async Task<int> GetCurrentEmployeeIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int currentUserId))
            {
                var emp = await _unitOfWork.UserRepo.TableNoTracking.FirstOrDefaultAsync(u => u.Id == currentUserId && !u.IsDeleted);
                if (emp != null)
                {
                    return emp.EmployeeId ?? 0;
                }
            }
            return 0;
        }
        public async Task<ApiResponseDto> GetEmployeeSearchListByNameOrEmailAsync(string name)
        {
            var employeeId = await GetCurrentEmployeeIdAsync();
            if (employeeId == 0) return ApiResponseDto.FailureStatus("Failed");

            var results = await (from e in _unitOfWork.EmployeeRepo.TableNoTracking
                          join u in _unitOfWork.UserRepo.TableNoTracking on e.Id equals u.EmployeeId
                           where !e.IsDeleted && e.Id != employeeId
                               && (e.FirstName.StartsWith(name) || e.LastName.StartsWith(name) || u.Email.StartsWith(name))
                            select new EmployeeShortResponseDto
                            {
                                Id = e.Id,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                Email = u.Email
                            })
                            .Take(10)
                            .ToListAsync();

            return ApiResponseDto.SuccessStatus(results);
        }
    }
}
