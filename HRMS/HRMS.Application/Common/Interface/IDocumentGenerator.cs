using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Constants.GeneralConstants;

namespace HRMS.Application.Common.Interface
{
    public interface IDocumentGenerator
    {
        byte[] GenerateDocument(string templatePath, DocTemplateModel dataModel, DocumentType type);
        Task<byte[]> GenerateDocumentAsync(string templatePath, DocTemplateModel dataModel, DocumentType type);

    }
}
