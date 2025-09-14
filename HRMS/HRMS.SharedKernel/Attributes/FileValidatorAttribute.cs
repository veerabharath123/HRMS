using HRMS.SharedKernel.Models.Common.Enum;
using HRMS.SharedKernel.Models.Request;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FileValidatorAttribute : ValidationAttribute
    {
        private readonly FileType[] _allowedTypes;
        private const int MaxSizeInBytes = 10 * 1024 * 1024; // 10 MB

        public FileValidatorAttribute(FileType[] allowedTypes)
        {
            _allowedTypes = allowedTypes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not FileRequestDto file)
                return new ValidationResult("Invalid file object");

            if (file.FileContent is null || file.FileContent.Length == 0)
                return new ValidationResult("File is empty");

            if (file.FileContent.Length > MaxSizeInBytes)
                return new ValidationResult("File exceeds maximum allowed size");

            // Extension check
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = _allowedTypes.Select(t => t switch
            {
                FileType.PDF => ".pdf",
                FileType.DOCX => ".docx",
                FileType.XLSX => ".xlsx",
                FileType.PNG => ".png",
                FileType.JPG => ".jpg",
                _ => ""
            }).ToArray();

            if (!allowedExtensions.Contains(ext))
                return new ValidationResult($"File type '{ext}' is not allowed");

            //// Optional: MIME / magic number check using MimeDetective
            //var inspector = new ContentInspectorBuilder().Build();
            //var results = inspector.Inspect(file.Content);
            //if (!results.Any(r => allowedExtensions.Contains(Path.GetExtension(r.FileType.Extension))))
            //    return new ValidationResult("File content does not match file type");

            return ValidationResult.Success;
        }
    }

}
