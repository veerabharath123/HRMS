using HRMS.SharedKernel.Models.Common.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class FileResponseDto: FileDto
    {
        public string FileNameWithExtension { get => $"{FileName}.{FileExtension}"; }
    }
    public class FileBase64ResponseDto 
    {
        public string FileBase64 { get; set; } = string.Empty;
        public Guid FileId { get; set; } 
    }
}
