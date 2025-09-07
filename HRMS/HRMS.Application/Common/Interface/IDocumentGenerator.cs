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
        byte[] GenerateWordDocument(string templatePath, DocTemplateModel dataModel);
        Task<byte[]> GenerateWordDocumentAsync(string templatePath, DocTemplateModel dataModel);
        byte[] GeneratePdfDocument(string templatePath, DocTemplateModel dataModel);
        Task<byte[]> GeneratePdfDocumentAsync(string templatePath, DocTemplateModel dataModel);


    }
}
