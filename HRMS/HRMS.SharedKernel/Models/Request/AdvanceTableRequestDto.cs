using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class AdvanceTableRequestDto
    {
        public FilterRequestDto? Filter { get; set; }
        public FilterGroupDto? FilterGroup { get; set; }
        public SortRequestDto? Sort { get; set; }
        public PaginationRequestDto? Pagination { get; set; }
    }
}
