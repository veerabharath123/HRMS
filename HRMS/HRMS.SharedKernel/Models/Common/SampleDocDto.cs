using HRMS.SharedKernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common
{
    public class SampleDocDto
    {
        [DocumentPlaceholder]
        public string Name { get; set; } = string.Empty;
        [DocumentPlaceholder]
        public string ToName { get; set; } = string.Empty;
        [DocumentPlaceholder]
        public string IssuedBy { get; set; } = string.Empty;
        [DocumentPlaceholder]
        public string Title { get; set; } = string.Empty;
        [DocumentPlaceholder("RegistererdDate")]
        public string RegistrationDate { get; set; } = string.Empty;
        [DocumentPlaceholder]
        public string DateOfBirth { get; set; } = string.Empty;
        [DocumentPlaceholder("para1")]
        public string ParagraphOne { get; set; } = string.Empty;
    }
}
