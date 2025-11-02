using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class EmployeeShortResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }
}
