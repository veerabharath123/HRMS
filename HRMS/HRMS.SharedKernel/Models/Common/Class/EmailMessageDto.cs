using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class EmailMessageDto
    {
        public List<EmailAddressDto> To { get; set; } = [];
        public List<EmailAddressDto> Cc { get; set; } = [];
        public List<EmailAddressDto> Bcc { get; set; } = [];
        public List<EmailAttachmentDto> Attachments { get; set; } = [];
        public string Body { get; set; } = string.Empty;
        public bool IsHtml { get; set; }
        public string Subject { get; set; } = string.Empty;

    }
    public class EmailAddressDto
    {
        private string _displayName = string.Empty;
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        public string DisplayName
        {
            get => string.IsNullOrWhiteSpace(_displayName) ? EmailAddress : _displayName;
            set => _displayName = value;
        }
    }
    public class EmailAttachmentDto
    {
        public byte[] FileContent { get; set; } = [];
        public string FileContentType { get; set; } = string.Empty;
        public string FileDisplayName { get; set; } = string.Empty;
    }
}
