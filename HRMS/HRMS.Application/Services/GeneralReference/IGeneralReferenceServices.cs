using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Services.GeneralReference
{
    public interface IGeneralReferenceServices
    {
        Task<List<BaseRefDto>> GetAllActiveGendersAsync();
        Task<List<BaseRefDto>> GetAllActiveMaritalStatusAsync();
    }
}