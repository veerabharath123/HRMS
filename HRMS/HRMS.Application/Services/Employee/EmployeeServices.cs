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
                                    FirstName = emp.FirstName,
                                    GuidId = emp.GuidId,
                                    HasPicture = emp.PhotoPictureId != null
                                }
                              )
                              .FilterBy(request.FilterGroup)
                              .SortBy(request.Sort)
                              .PaginateAsync(request.Pagination);

            return ApiResponseDto.SuccessStatus(result);
        }
        public async Task<ApiResponseDto> GetEmployeeByIdAsync(int employeeId)
        {
            var emp = await _unitOfWork.EmployeeRepo.TableNoTracking
                        .Where(e => !e.IsDeleted && e.Id == employeeId)
                        .Select(emp => new EmployeeDto
                        {
                            Bio = emp.Bio,
                            RelievingDate = emp.RelievingDate,
                            JoiningDate = emp.JoiningDate,
                            ResignationDate = emp.ResignationDate,
                            FirstName = emp.FirstName,
                            LastName = emp.LastName,
                            MiddleName = emp.MiddleName,
                            BirthDate = emp.BirthDate,
                            ReportingManagerId = emp.ReportingManagerId,
                            DepartmentId = emp.DepartmentId,
                            DesignationId = emp.DesignationId,
                            MaritalStatusId = emp.MaritalStatusId ?? 0,
                            GenderId = emp.GenderId,
                            
                        }).FirstOrDefaultAsync();

            if (emp == null) return ApiResponseDto.FailureStatus("Employee not found.");
            
            return ApiResponseDto.SuccessStatus(new UpdateEmployeeRequestDto { Employee = emp });
        }

        public async Task<ApiResponseDto> GetEmployeeDetailsAsync(int employeeId)
        {
            var result = await (from emp in _unitOfWork.EmployeeRepo.TableNoTracking
                                join dept in _unitOfWork.DepartmentRepo.TableNoTracking on emp.DepartmentId equals dept.Id
                                into deptJoin from dept in deptJoin.DefaultIfEmpty()
                                join des in _unitOfWork.DesignationRepo.TableNoTracking on emp.DesignationId equals des.Id
                                into desJoin from des in desJoin.DefaultIfEmpty()
                                join ms in _unitOfWork.GeneralReferenceRepo.TableNoTracking.Where(ms => ms.Category == "MartialStatus") on emp.MaritalStatusId equals ms.Id
                                into msJoin from ms in msJoin.DefaultIfEmpty()
                                join g in _unitOfWork.GeneralReferenceRepo.TableNoTracking.Where(g => g.Category == "Gender") on emp.GenderId equals g.Id
                                into gJoin from g in gJoin.DefaultIfEmpty()
                                join rm in _unitOfWork.EmployeeRepo.TableNoTracking on emp.ReportingManagerId equals rm.Id
                                into rmJoin from rm in rmJoin.DefaultIfEmpty()
                                join usr in _unitOfWork.UserRepo.TableNoTracking on emp.Id equals usr.EmployeeId
                                into usrJoin from usr in usrJoin.DefaultIfEmpty()

                                let user = _unitOfWork.UserRepo.TableNoTracking.FirstOrDefault(u => !string.IsNullOrWhiteSpace(emp.Email) && u.Email.ToLower() == emp.Email.ToLower())

                                where emp.Id == employeeId && !emp.IsDeleted

                                select new EmployeeDetailsDto
                                {
                                    EmployeeId = emp.Id,
                                    Bio = emp.Bio,
                                    RelievingDate = emp.RelievingDate,
                                    JoiningDate = emp.JoiningDate,
                                    ResignationDate = emp.ResignationDate,
                                    FirstName = emp.FirstName,
                                    LastName = emp.LastName,
                                    MiddleName = emp.MiddleName,
                                    BirthDate = emp.BirthDate,
                                    Department = dept != null ? dept.Name : string.Empty,
                                    Designation = des != null ? des.Name : string.Empty,
                                    ReportingManager = rm != null ? rm.FullName : string.Empty,
                                    MartialStatus = ms != null ? ms.Value : string.Empty,
                                    Gender = g != null ? g.Value : string.Empty,
                                    Email = emp.Email,
                                    UserExists = usr != null || user == null,
                                    HasPicture = emp.PhotoPictureId != null,
                                    GuidId = emp.GuidId,
                                    UserName = usr == null && user != null ? user.UserName : string.Empty
                                }
                              )
                              .FirstOrDefaultAsync();

            if(result == null) return ApiResponseDto.FailureStatus("Employee not found.");

            var empDetails = new EmployeeDetaisResponseDto { EmployeeDetails = result };
            return ApiResponseDto.SuccessStatus(empDetails);
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
        public async Task<FileResponseDto?> GetEmployeeImageAsync(Guid id)
        {
            var employee = await _unitOfWork.EmployeeRepo.TableNoTracking.FirstOrDefaultAsync(e => e.GuidId == id && !e.IsDeleted);

            if (employee is null || !employee.PhotoPictureId.HasValue) return null;

            return await _fileServices.GetFileBytesByStoredFileIdAsync(employee.PhotoPictureId.Value);
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
