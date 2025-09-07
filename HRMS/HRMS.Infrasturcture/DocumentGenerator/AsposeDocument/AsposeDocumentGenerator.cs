using Aspose.Words;
using Aspose.Words.Replacing;
using HRMS.Domain.Common;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
{
    /// <summary>
    /// Provides methods to generate documents from a template using <see cref="DocTemplateModel"/>.
    /// Supports text, images, and table placeholders, and can export in various <see cref="SaveFormat"/> formats.
    /// </summary>
    public class AsposeDocumentGenerator
    {
        /// <summary>
        /// Generates a document from a template file and a <see cref="DocTemplateModel"/>.
        /// </summary>
        /// <param name="templatePath">The path to the Word template file.</param>
        /// <param name="model">The template model containing text, images, and tables.</param>
        /// <param name="format">The output format (default: <see cref="SaveFormat.Docx"/>).</param>
        /// <returns>A byte array representing the generated document.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the template file does not exist.</exception>
        public static byte[] GenerateDocument(string templatePath, DocTemplateModel model, SaveFormat format = SaveFormat.Docx)
        {
            var doc = BuildDocument(templatePath, model);
            return SaveToStream(doc, format);
        }

        /// <summary>
        /// Asynchronously generates a document from a template file and a <see cref="DocTemplateModel"/>.
        /// </summary>
        /// <param name="templatePath">The path to the Word template file.</param>
        /// <param name="model">The template model containing text, images, and tables.</param>
        /// <param name="format">The output format (default: <see cref="SaveFormat.Docx"/>).</param>
        /// <returns>A task representing the asynchronous operation. The result is a byte array of the generated document.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the template file does not exist.</exception>
        public async static Task<byte[]> GenerateDocumentAsync(string templatePath, DocTemplateModel model, SaveFormat format = SaveFormat.Docx)
        {
            var doc = BuildDocument(templatePath, model);
            return await Task.Run(() => SaveToStream(doc, format));
        }

        #region Private Helpers

        /// <summary>
        /// Builds a document from a template path and a <see cref="DocTemplateModel"/>.
        /// Inserts text, images, and tables based on the model.
        /// </summary>
        private static Document BuildDocument(string templatePath, DocTemplateModel model)
        {
            if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
                throw new FileNotFoundException($"Template not found at {templatePath}");

            var doc = new Document(templatePath);

            InsertTextFields(doc, model);
            InsertImages(doc, model);
            InsertTables(doc, model);

            return doc;
        }

        /// <summary>
        /// Saves a document to a byte array using the specified format.
        /// </summary>
        private static byte[] SaveToStream(Document doc, SaveFormat format)
        {
            using var stream = new MemoryStream();
            doc.Save(stream, format);
            return stream.ToArray();
        }

        /// <summary>
        /// Replaces text placeholders in the document with values from <see cref="DocTemplateModel.TextFields"/>.
        /// </summary>
        private static void InsertTextFields(Document doc, DocTemplateModel model)
        {
            foreach (var field in model.TextFields)
            {
                var options = new FindReplaceOptions
                {
                    ReplacingCallback = new ReplaceWithTextHandler(field)
                };

                var placeholder = $"{model.StartIndicator}{field.Name}{model.EndIndicator}";
                doc.Range.Replace(placeholder, field.Value ?? string.Empty, options);
            }
        }

        /// <summary>
        /// Replaces image placeholders in the document with images from <see cref="DocTemplateModel.ImageFields"/>.
        /// </summary>
        private static void InsertImages(Document doc, DocTemplateModel model)
        {
            foreach (var field in model.ImageFields)
            {
                var placeholder = $"{model.StartIndicator}{field.Name}{model.EndIndicator}";

                if (!string.IsNullOrEmpty(field.ImagePath) && File.Exists(field.ImagePath))
                {
                    ReplacePlaceholderWithImage(doc, placeholder, field.ImagePath, field.ImageDimension);
                }
                else if (field.ImageStream is not null)
                {
                    ReplacePlaceholderWithImage(doc, placeholder, field.ImageStream, field.ImageDimension);
                }
            }
        }

        /// <summary>
        /// Replaces table placeholders in the document with tables from <see cref="DocTemplateModel.TableFields"/>.
        /// </summary>
        private static void InsertTables(Document doc, DocTemplateModel model)
        {
            foreach (var table in model.TableFields)
            {
                string placeholder = $"{model.StartIndicator}{table.Name}{model.EndIndicator}";
                var options = new FindReplaceOptions
                {
                    ReplacingCallback = new ReplaceWithTableHandler(doc, table)
                };

                doc.Range.Replace(placeholder, string.Empty, options);
            }
        }

        /// <summary>
        /// Replaces a placeholder with an image from a file path.
        /// </summary>
        private static void ReplacePlaceholderWithImage(Document doc, string placeholder, string imagePath, ImageFieldDimension dimension)
        {
            var options = new FindReplaceOptions
            {
                ReplacingCallback = new ReplaceWithImageHandler(imagePath, dimension.Width, dimension.Height)
            };
            doc.Range.Replace(placeholder, string.Empty, options);
        }

        /// <summary>
        /// Replaces a placeholder with an image from a stream.
        /// </summary>
        private static void ReplacePlaceholderWithImage(Document doc, string placeholder, Stream imageStream, ImageFieldDimension dimension)
        {
            var options = new FindReplaceOptions
            {
                ReplacingCallback = new ReplaceWithImageHandler(imageStream, dimension.Width, dimension.Height)
            };
            doc.Range.Replace(placeholder, string.Empty, options);
        }

        #endregion
    }

}

