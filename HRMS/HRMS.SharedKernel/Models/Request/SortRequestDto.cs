using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class SortRequestDto
    {
        public List<SortOptionDto> SortOptions { get; set; } = [];
    }
}
