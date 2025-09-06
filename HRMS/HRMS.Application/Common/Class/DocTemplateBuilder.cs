using HRMS.Domain.Common;
using HRMS.Domain.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Records.DocImageInfoRecords;

namespace HRMS.Application.Common.Class
{
    public class DocTemplateBuilder
    {
        private readonly DocTemplateModel _docTemplate;

        private DocTemplateBuilder(DocTemplateModel model)
        {
            _docTemplate = model;
        }

        public static DocTemplateBuilder Create(string startIndicator = "<", string endIndicator = ">")
        {
            var model = new DocTemplateModel(startIndicator, endIndicator);
            return new DocTemplateBuilder(model);
        }

        // Add text fields from model
        public DocTemplateBuilder WithTextFrom<T>(T model)
        {
            var clone = CloneModel(_docTemplate);
            clone.LoadFromModel(model); // uses your extension
            return new DocTemplateBuilder(clone);
        }
        public DocTemplateBuilder WithText(
            string placeholder,
            string value,
            DocTextStyle? style = null)
        {
            var clone = CloneModel(_docTemplate);

            clone.TextFields.Add(new DocTemplateTextField
            {
                Name = placeholder,
                Value = value,
                Style = style,
            });
            return new DocTemplateBuilder(clone);
        }

        // Add image from path
        public DocTemplateBuilder WithImage(string placeholder, string imagePath, ImageDimension? dimension = null)
        {
            var clone = CloneModel(_docTemplate);
            clone.LoadImages(placeholder, imagePath, dimension);
            return new DocTemplateBuilder(clone);
        }

        // Add image from stream
        public DocTemplateBuilder WithImage(string placeholder, Stream imageStream, ImageDimension dimension)
        {
            var clone = CloneModel(_docTemplate);
            clone.LoadImages(placeholder, imageStream, dimension);
            return new DocTemplateBuilder(clone);
        }
        // Add multiple images at once
        public DocTemplateBuilder WithImages(IEnumerable<DocImageInfoRec> images)
        {
            var clone = CloneModel(_docTemplate);

            foreach (var img in images)
            {
                if (img.Path is not null)
                    clone.LoadImages(img.Placeholder, img.Path, img.Dimension);
                else if (img.Stream is not null)
                    clone.LoadImages(img.Placeholder, img.Stream, img.Dimension);
            }

            return new DocTemplateBuilder(clone);
        }

        // Add table
        public DocTemplateBuilder WithTable(string placeholder, List<string[]> rows, int[]? columnWidths = null)
        {
            var clone = CloneModel(_docTemplate);
            clone.TableFields.Add(new DocTemplateTableField
            {
                Name = placeholder,
                Rows = rows,
                ColumnWidths = columnWidths
            });
            return new DocTemplateBuilder(clone);
        }

        // Finalize
        public DocTemplateModel Build()
        {
            return _docTemplate;
        }

        // Clone helper for immutability
        private static DocTemplateModel CloneModel(DocTemplateModel source)
        {
            return new DocTemplateModel(source.StartIndicator, source.EndIndicator)
            {
                TextFields = [.. source.TextFields],
                ImageFields = [.. source.ImageFields],
                TableFields = [.. source.TableFields]
            };
        }
    }

}
