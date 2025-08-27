using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class BaseRefDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
