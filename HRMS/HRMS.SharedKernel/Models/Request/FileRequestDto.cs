using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class FileRequestDto: FileDto
    {
    }
    public class UploadFileRequestDto: FileDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}
