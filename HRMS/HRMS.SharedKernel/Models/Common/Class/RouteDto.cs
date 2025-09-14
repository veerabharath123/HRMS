using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class RouteDto
    {
        public string RouteId { get; set; } = string.Empty;
        public string ClusterId { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
