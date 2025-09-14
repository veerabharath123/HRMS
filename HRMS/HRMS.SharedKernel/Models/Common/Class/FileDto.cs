using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class FileDto
    {
        [Required]
        public byte[]? FileContent { get; set; }
        [Required]
        public string FileName { get; set; } = string.Empty;
        [Required]
        public string FileContentType { get; set; } = string.Empty;
        [Required]
        public string FileExtension { get; set; } = string.Empty;
    }
}
