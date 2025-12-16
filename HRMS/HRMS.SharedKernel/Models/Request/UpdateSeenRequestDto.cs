using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class UpdateSeenRequestDto
    {
        [Required]
        public DateTime? TillRead { get; set; }
        [Required]
        public int ConversationId { get; set; }
    }
}
