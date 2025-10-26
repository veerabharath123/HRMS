using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Services
{
    public interface IReferenceServices
    {
        Task<List<BaseRefDto>> GetAllActiveDepartmentsAsync();
    }
}