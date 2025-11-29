using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Response;

namespace HRMS.Application.Services
{
    public interface IReferenceServices
    {
        Task<List<BaseRefDto>> GetAllActiveDepartmentsAsync();
        Task<List<BaseRefDto>> GetAllActiveDesignationsAsync();
        Task<List<BaseRefDto>> GetAllActiveGendersAsync();
        Task<List<BaseRefDto>> GetAllActiveMaritalStatusAsync();
        Task<List<ModulesResponseDto>> GetAllActiveModulesAsync();
    }
}