using Aspose.Words;
using HRMS.Application.Common.Interface;
using HRMS.Domain.Common;
using static HRMS.Domain.Constants.GeneralConstants;
using HRMS.Infrastructure.DocumentGenerator.AsposeDocument;

namespace HRMS.Infrastructure.DocumentGenerator
{
    public class DocumentGenerator: IDocumentGenerator
    {
        public byte[] GenerateDocument(string templatePath, DocTemplateModel dataModel, DocumentType type)
        {
            var format = MapDocumentType(type);

            return AsposeDocumentGenerator.GenerateDocument(templatePath, dataModel, format);
        }
        public Task<byte[]> GenerateDocumentAsync(string templatePath, DocTemplateModel dataModel, DocumentType type)
        {
            var format = MapDocumentType(type);

            return AsposeDocumentGenerator.GenerateDocumentAsync(templatePath, dataModel, format);
        }
        private static SaveFormat MapDocumentType(DocumentType type) =>
                type switch
                {
                    DocumentType.Word => Aspose.Words.SaveFormat.Docx,
                    DocumentType.Pdf => Aspose.Words.SaveFormat.Pdf,
                    _ => throw new NotSupportedException($"Unsupported document type: {type}")
                };
    }
}
