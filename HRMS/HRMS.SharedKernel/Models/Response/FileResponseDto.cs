using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class FileResponseDto
    {
        public byte[]? FileContent { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileNameWithExtension { get => $"{FileName}.{FileExtension}"; }
        public string FileContentType { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
    }
}
