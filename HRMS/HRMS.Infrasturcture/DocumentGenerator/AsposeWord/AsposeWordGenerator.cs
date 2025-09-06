using Aspose.Words;
using Aspose.Words.Replacing;
using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace HRMS.Infrasturcture.DocumentGenerator.AsposeWord
{
    public class AsposeWordGenerator
    {
        public static byte[] GenerateWordDoc(string templatePath, DocTemplateModel model, SaveFormat format = SaveFormat.Docx)
        {
            Document doc = new(templatePath);

            foreach (var field in model.TextFields)
            {
                FindReplaceOptions options = new()
                {
                    ReplacingCallback = new ReplaceWithStyledTextHandler(field)
                };

                doc.Range.Replace($"{model.StartIndicator}{field.Name}{model.EndIndicator}", field.Value ?? string.Empty, options);
            }
            foreach (var field in model.ImageFields)
            {
                var placeholder = $"{model.StartIndicator}{field.Name}{model.EndIndicator}";
                
                string? imagePath = field.ImagePath;

                if (string.IsNullOrEmpty(imagePath) && field.ImageStream is not null)
                {
                    ReplacePlaceholderWithImage(doc, placeholder, field.ImageStream, field.ImageDimension);
                }

                else if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    ReplacePlaceholderWithImage(doc, placeholder, imagePath, field.ImageDimension);
                }
            }

            // Save output document
            using var ms = new MemoryStream();
            doc.Save(ms, format); // or PDF if you need
            return ms.ToArray();
        }
        private static void ReplacePlaceholderWithImage(Document doc, string placeholder, string imagePath, ImageDimension dimension)
        {
            FindReplaceOptions options = new()
            {
                ReplacingCallback = new ReplaceWithImageHandler(imagePath, dimension.Width, dimension.Height)
            };
            doc.Range.Replace(placeholder, string.Empty, options);
        }
        private static void ReplacePlaceholderWithImage(Document doc, string placeholder, Stream imageStream, ImageDimension dimension)
        {
            FindReplaceOptions options = new()
            {
                ReplacingCallback = new ReplaceWithImageHandler(imageStream, dimension.Width, dimension.Height)
            };
            doc.Range.Replace(placeholder, string.Empty, options);
        }
    }
}
