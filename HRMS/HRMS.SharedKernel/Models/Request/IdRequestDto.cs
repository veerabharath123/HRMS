using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class IdRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
    public class GuidIdRequestDto
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
