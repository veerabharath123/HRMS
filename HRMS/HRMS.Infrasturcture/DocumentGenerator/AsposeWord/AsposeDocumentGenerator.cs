using Aspose.Words;
using Aspose.Words.Replacing;
using Aspose.Words.Tables;
using HRMS.Domain.Common;
using HRMS.Infrasturcture.DocumentGenerator.AsposeWord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeWord
{
    public class AsposeDocumentGenerator
    {
        public static byte[] GenerateDocument(string templatePath, DocTemplateModel model, SaveFormat format = SaveFormat.Docx)
        {
            var doc = BuildDocument(templatePath, model);

            return SaveToStream(doc, format);
        }
        public async static Task<byte[]> GenerateDocumentAsync(string templatePath, DocTemplateModel model, SaveFormat format = SaveFormat.Docx)
        {
            var doc = BuildDocument(templatePath, model);

            return await Task.Run(() => SaveToStream(doc, format));
        }

        #region Private Helpers

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

        private static byte[] SaveToStream(Document doc, SaveFormat format)
        {
            using var stream = new MemoryStream();
            doc.Save(stream, format);
            return stream.ToArray();
        }

        private static void InsertTextFields(Document doc, DocTemplateModel model)
        {
            foreach (var field in model.TextFields)
            {
                var options = new FindReplaceOptions
                {
                    ReplacingCallback = new ReplaceWithStyledTextHandler(field)
                };

                var placeholder = $"{model.StartIndicator}{field.Name}{model.EndIndicator}";
                doc.Range.Replace(placeholder, field.Value ?? string.Empty, options);
            }
        }

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

        //private static void InsertTables(Document doc, DocTemplateModel model)
        //{
        //    foreach (var table in model.TableFields)
        //    {
        //        var builder = new DocumentBuilder(doc);

        //        // Move to placeholder or bookmark
        //        builder.MoveToBookmark(table.Name);

        //        Table aspTable = builder.StartTable();
        //        foreach (var row in table.Rows)
        //        {
        //            foreach (var cell in row)
        //            {
        //                builder.InsertCell();
        //                builder.Write(cell);
        //            }
        //            builder.EndRow();
        //        }

        //        builder.EndTable();
        //    }
        //}
        private static void InsertTables(Document doc, DocTemplateModel model)
        {
            var builder = new DocumentBuilder(doc);

            foreach (var table in model.TableFields)
            {
                // Find placeholder run that contains <tableName>
                var run = doc.GetChildNodes(NodeType.Run, true)
                             .OfType<Run>()
                             .FirstOrDefault(r => r.Text.Contains($"{model.StartIndicator}{table.Name}{model.EndIndicator}"));

                if (run == null)
                    continue; // skip if no placeholder found

                // Move builder to placeholder
                builder.MoveTo(run);

                // Clear the placeholder text
                run.Text = string.Empty;

                // Build the table
                Table aspTable = builder.StartTable();
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row)
                    {
                        builder.InsertCell();
                        builder.Write(cell);
                    }
                    builder.EndRow();
                }
                builder.EndTable();
            }
        }


        private static void ReplacePlaceholderWithImage(Document doc, string placeholder, string imagePath, ImageDimension dimension)
        {
            var options = new FindReplaceOptions
            {
                ReplacingCallback = new ReplaceWithImageHandler(imagePath, dimension.Width, dimension.Height)
            };
            doc.Range.Replace(placeholder, string.Empty, options);
        }

        private static void ReplacePlaceholderWithImage(Document doc, string placeholder, Stream imageStream, ImageDimension dimension)
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

