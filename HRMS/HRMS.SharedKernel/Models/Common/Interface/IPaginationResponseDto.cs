using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Interface
{
    public interface IPaginationResponseDto
    {
        int PageNumber { get; }
        int TotalPages { get; }
        int FirstPageToShow { get; }
        int LastPageToShow { get; }
        int TotalItems { get; }
    }
}
