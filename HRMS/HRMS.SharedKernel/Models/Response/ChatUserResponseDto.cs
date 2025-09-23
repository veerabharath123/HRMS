using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class ChatUserResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
