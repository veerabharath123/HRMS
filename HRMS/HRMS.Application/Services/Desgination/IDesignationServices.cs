using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Services.Desgination
{
    public interface IDesignationServices
    {
        Task<List<BaseRefDto>> GetAllActiveDesignationsAsync();
    }
}