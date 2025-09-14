using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class ProxyConfigResponseDto
    {
        public List<RouteDto> Routes { get; set; } = new();
        public List<ClusterDto> Clusters { get; set; } = new();
    }
}
