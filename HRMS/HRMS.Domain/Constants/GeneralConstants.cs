using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Constants
{
    public class GeneralConstants
    {
        public const string WEEKLY_EXPENSE_NOTE = "Your total expenses that you spent this week is {0}";
        public const string USER_ALREADY_EXISTS_MSG = "User with this name {0} already exists, please choose different name";
        public const string USER_PERMISSION_KEY = "UserPermissions";
        public const string QR_NO_DATA_MSG = "QR data cannot be null or empty.";
        public const string WORD_SAMPLE_PARA = "Organizations that prioritize customer experience and leverage data-driven insights are better positioned to make impactful decisions. As technology continues to reshape industries, collaboration and continuous learning remain key drivers of sustainable growth. ";
        public const string CONTENT_TYPE_DOCX = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string CONTENT_TYPE_PDF = "application/pdf";
        public const string SAMPE_TEMPLATE_PATH = @"{0}Resources\DocumentTemplates\Sample Template.docx";
        public const string SAMPE_SIGNATURE_PATH_PNG = @"{0}Resources\Signatures\{1}.png";
        public enum DependencyInjectionTypes
        {
            Transient,
            Scoped,
            Singleton
        }
        public enum DocumentType
        {
            Word,
            Pdf
        }
    }
}
