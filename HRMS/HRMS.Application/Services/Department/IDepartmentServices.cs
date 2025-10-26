using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Services
{
    public interface IDepartmentServices
    {
        Task<List<BaseRefDto>> GetAllActiveDepartmentsAsync();
    }
}