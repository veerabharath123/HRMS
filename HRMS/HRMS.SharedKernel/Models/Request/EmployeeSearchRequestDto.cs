using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class EmployeeSearchRequestDto
    {
        [Required]
        public string SearchText { get; set; } = string.Empty;
    }
}
