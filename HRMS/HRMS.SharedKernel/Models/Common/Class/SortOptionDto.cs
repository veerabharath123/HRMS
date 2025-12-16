using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class SortOptionDto 
    {
        public string PropertyName { get; set; } = string.Empty;
        public bool Descending { get; set; }
    }
}
