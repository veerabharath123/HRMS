using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface IDocumentGenerator
    {
        byte[] GenerateWord(string templatePath, DocTemplateModel dataModel);
        byte[] GeneratePdf(string templatePath, DocTemplateModel dataModel);
        byte[] GenerateExcel(string templatePath, DocTemplateModel dataModel);

    }
}
