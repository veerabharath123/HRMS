using HRMS.SharedKernel.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class MessageResponseDto:MessageRequestDto
    {
        public bool IsMe { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
