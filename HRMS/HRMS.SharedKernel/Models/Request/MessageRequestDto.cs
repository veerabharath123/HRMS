using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class MessageRequestDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public DateTime Date { get; set; } 
    }
}
