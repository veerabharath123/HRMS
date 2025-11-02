using HRMS.SharedKernel.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services.File
{
    public interface IFileServices
    {
        Task<ApiResponseDto> ProcessFileMaintenanceAsync(CancellationToken cancellationToken = default);
    }
}
