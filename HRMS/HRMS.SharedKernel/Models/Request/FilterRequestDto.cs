using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class FilterRequestDto 
    {
        public List<FilterOptionDto> Filters { get; set; } = [];
    }
    public class FilterGroupDto
    {
        public List<FilterOptionDto>? Filters { get; set; } = [];
        public List<FilterGroupDto>? Groups { get; set; } = [];

        // How to combine filters within this group
        public string Operator { get; set; } = "And";
    }
}
