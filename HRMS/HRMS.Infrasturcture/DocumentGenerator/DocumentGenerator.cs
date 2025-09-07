using Aspose.Words;
using HRMS.Application.Common.Interface;
using HRMS.Domain.Common;
using static HRMS.Domain.Constants.GeneralConstants;
using HRMS.Infrastructure.DocumentGenerator.AsposeDocument;

namespace HRMS.Infrastructure.DocumentGenerator
{
    /// <summary>
    /// Provides methods to generate Word and PDF documents from templates and data models.
    /// Implements <see cref="IDocumentGenerator"/>.
    /// </summary>
    public class DocumentGenerator : IDocumentGenerator
    {
        /// <summary>
        /// Generates a Word document (.docx) from the specified template and data model.
        /// </summary>
        /// <param name="templatePath">Path to the Word template file.</param>
        /// <param name="dataModel">Data model containing text, images, and tables.</param>
        /// <returns>Byte array representing the generated Word document.</returns>
        public byte[] GenerateWordDocument(string templatePath, DocTemplateModel dataModel)
        {
            return AsposeDocumentGenerator.GenerateDocument(templatePath, dataModel, SaveFormat.Docx);
        }

        /// <summary>
        /// Asynchronously generates a Word document (.docx) from the specified template and data model.
        /// </summary>
        /// <param name="templatePath">Path to the Word template file.</param>
        /// <param name="dataModel">Data model containing text, images, and tables.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the generated Word document as a byte array.</returns>
        public Task<byte[]> GenerateWordDocumentAsync(string templatePath, DocTemplateModel dataModel)
        {
            return AsposeDocumentGenerator.GenerateDocumentAsync(templatePath, dataModel, SaveFormat.Docx);
        }

        /// <summary>
        /// Generates a PDF document from the specified template and data model.
        /// </summary>
        /// <param name="templatePath">Path to the Word template file.</param>
        /// <param name="dataModel">Data model containing text, images, and tables.</param>
        /// <returns>Byte array representing the generated PDF document.</returns>
        public byte[] GeneratePdfDocument(string templatePath, DocTemplateModel dataModel)
        {
            return AsposeDocumentGenerator.GenerateDocument(templatePath, dataModel, SaveFormat.Pdf);
        }

        /// <summary>
        /// Asynchronously generates a PDF document from the specified template and data model.
        /// </summary>
        /// <param name="templatePath">Path to the Word template file.</param>
        /// <param name="dataModel">Data model containing text, images, and tables.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the generated PDF document as a byte array.</returns>
        public Task<byte[]> GeneratePdfDocumentAsync(string templatePath, DocTemplateModel dataModel)
        {
            return AsposeDocumentGenerator.GenerateDocumentAsync(templatePath, dataModel, SaveFormat.Pdf);
        }
    }

}
