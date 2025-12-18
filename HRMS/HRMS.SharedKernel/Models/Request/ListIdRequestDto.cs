using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class ListIdRequestDto
    {
        public List<int> IdList { get; set; } = [];
    }
    public class ListGuidIdRequestDto
    {
        public List<Guid> IdList { get; set; } = [];
    }
}
