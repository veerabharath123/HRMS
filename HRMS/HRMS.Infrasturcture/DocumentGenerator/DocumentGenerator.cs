using HRMS.Application.Common.Interface;
using HRMS.Domain.Common;
using HRMS.Infrasturcture.DocumentGenerator.AsposeWord;
using HRMS.SharedKernel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrasturcture.DocumentGenerator
{
    public class DocumentGenerator: IDocumentGenerator
    {
        public DocumentGenerator()
        {
        }

        public byte[] GenerateWord(string templatePath, DocTemplateModel dataModel)
        {
            return AsposeWordGenerator.GenerateWordDoc(templatePath, dataModel, Aspose.Words.SaveFormat.Docx);
        }
        public byte[] GeneratePdf(string templatePath, DocTemplateModel dataModel)
        {
            return AsposeWordGenerator.GenerateWordDoc(templatePath, dataModel, Aspose.Words.SaveFormat.Pdf);
        }
        public byte[] GenerateExcel(string templatePath, DocTemplateModel dataModel)
        {
            throw new NotImplementedException();
        }
    }
}
