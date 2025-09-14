using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class ClusterDto
    {
        public string ClusterId { get; set; } = string.Empty;
        public Dictionary<string, string> Destinations { get; set; } = new();
    }
}
