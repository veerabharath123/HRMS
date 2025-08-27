using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class JwtUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}
