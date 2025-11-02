using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;

namespace HRMS.Application.Services.Employee
{
    public interface IEmployeeServices
    {
        Task<ApiResponseDto> GetPaginatedEmployeesShortAsync(AdvanceTableRequestDto request);
    }
}