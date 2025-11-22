using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class EmpImageResponseDto
    {
        public int Id { get; set; }
        public string ImageBase64 { get; set; } = string.Empty;
    }
}
