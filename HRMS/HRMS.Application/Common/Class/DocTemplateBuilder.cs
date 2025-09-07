using HRMS.Domain.Common;
using HRMS.Domain.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMS.Domain.Records.DocTemplateRecords;

namespace HRMS.Application.Common.Class
{
    /// <summary>
    /// A builder class for constructing <see cref="DocTemplateModel"/> instances
    /// using a fluent API. Supports adding text fields, images, and tables.
    /// </summary>
    public class DocTemplateBuilder
    {
        private readonly DocTemplateModel _docTemplate;

        private DocTemplateBuilder(DocTemplateModel model)
        {
            _docTemplate = model;
        }

        /// <summary>
        /// Creates a new <see cref="DocTemplateBuilder"/> instance.
        /// </summary>
        /// <param name="startIndicator">
        /// The start marker for placeholders in the document template. Default is "&lt;".
        /// </param>
        /// <param name="endIndicator">
        /// The end marker for placeholders in the document template. Default is "&gt;".
        /// </param>
        /// <returns>A new instance of <see cref="DocTemplateBuilder"/>.</returns>
        public static DocTemplateBuilder Create(string startIndicator = "<", string endIndicator = ">")
        {
            var model = new DocTemplateModel(startIndicator, endIndicator);
            return new DocTemplateBuilder(model);
        }

        /// <summary>
        /// Adds a single text field to the template.
        /// </summary>
        /// <param name="placeholder">The placeholder name to be replaced in the document.</param>
        /// <param name="value">The text value that will replace the placeholder.</param>
        /// <param name="style">Optional text style (e.g., bold, italic, font size).</param>
        /// <returns>A new builder instance with the added text field.</returns>
        public DocTemplateBuilder WithText(
            string placeholder,
            string value,
            DocTextStyle? style = null)
        {
            var textField = new DocTemplateTextField
            {
                Name = placeholder,
                Value = value,
                Style = style,
            };
            return WithTextField(textField);
        }

        /// <summary>
        /// Adds a pre-built <see cref="DocTemplateTextField"/> to the template.
        /// </summary>
        /// <param name="textField">The text field to add.</param>
        /// <returns>A new builder instance with the added text field.</returns>
        public DocTemplateBuilder WithTextField(DocTemplateTextField textField)
        {
            var clone = CloneModel(_docTemplate);
            clone.TextFields.Add(textField);
            return new DocTemplateBuilder(clone);
        }

        /// <summary>
        /// Adds multiple pre-built <see cref="DocTemplateTextField"/>s to the template.
        /// </summary>
        /// <param name="textFields">The collection of text fields to add.</param>
        /// <returns>A new builder instance with the added text fields.</returns>
        public DocTemplateBuilder WithTextFields(IEnumerable<DocTemplateTextField> textFields)
        {
            var clone = CloneModel(_docTemplate);
            clone.TextFields.AddRange(textFields);
            return new DocTemplateBuilder(clone);
        }

        /// <summary>
        /// Adds an image to the template using a file path.
        /// </summary>
        /// <param name="placeholder">The placeholder name to be replaced in the document.</param>
        /// <param name="imagePath">The file path to the image.</param>
        /// <param name="dimension">Optional dimensions (width/height) for the image.</param>
        /// <returns>A new builder instance with the added image.</returns>
        public DocTemplateBuilder WithImage(string placeholder, string imagePath, ImageFieldDimension? dimension = null)
        {
            var imageField = new DocTemplateImageField
            {
                Name = placeholder,
                ImagePath = imagePath,
                ImageDimension = dimension ?? new()
            };
            return WithImageField(imageField);
        }

        /// <summary>
        /// Adds an image to the template using a stream.
        /// </summary>
        /// <param name="placeholder">The placeholder name to be replaced in the document.</param>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="dimension">The image dimensions (width/height).</param>
        /// <returns>A new builder instance with the added image.</returns>
        public DocTemplateBuilder WithImage(string placeholder, Stream imageStream, ImageFieldDimension dimension)
        {
            var imageField = new DocTemplateImageField
            {
                Name = placeholder,
                ImageStream = imageStream,
                ImageDimension = dimension ?? new()
            };
            return WithImageField(imageField);
        }
        /// <summary>
        /// Adds a pre-built <see cref="DocTemplateImageField"/> to the template.
        /// This method clones the internal model (immutably) and returns a new builder instance.
        /// </summary>
        /// <param name="imageField">The image field to add (placeholder, path/stream, dimensions).</param>
        /// <returns>A new <see cref="DocTemplateBuilder"/> instance containing the added image field.</returns>
        private DocTemplateBuilder WithImageField(DocTemplateImageField imageField)
        {
            var clone = CloneModel(_docTemplate);
            clone.ImageFields.Add(imageField);
            return new DocTemplateBuilder(clone);
        }

        /// <summary>
        /// Adds multiple pre-built <see cref="DocTemplateImageField"/> entries to the template at once.
        /// This is useful when you prepare image fields elsewhere and want to add them in bulk.
        /// </summary>
        /// <param name="imageFields">A collection of <see cref="DocTemplateImageField"/> to add.</param>
        /// <returns>A new <see cref="DocTemplateBuilder"/> instance containing the added image fields.</returns>
        private DocTemplateBuilder WithImageFields(IEnumerable<DocTemplateImageField> imageFields)
        {
            var clone = CloneModel(_docTemplate);
            clone.ImageFields.AddRange(imageFields);
            return new DocTemplateBuilder(clone);
        }

        /// <summary>
        /// Adds multiple images to the template at once.
        /// Supports both file paths and streams.
        /// </summary>
        /// <param name="images">The collection of images to add, each with a placeholder, source, and dimensions.</param>
        /// <returns>A new builder instance with the added images.</returns>
        public DocTemplateBuilder WithImages(IEnumerable<DocImageInfoRec> images)
        {
            var clone = CloneModel(_docTemplate);
            List<DocTemplateImageField> imageFields = [];

            foreach (var img in images)
            {
                if (img.Path is not null)
                    imageFields.Add(new DocTemplateImageField
                    {
                        Name = img.Placeholder,
                        ImagePath = img.Path,
                        ImageDimension = img.Dimension ?? new()
                    });

                else if (img.Stream is not null)
                    imageFields.Add(new DocTemplateImageField
                    {
                        Name = img.Placeholder,
                        ImageStream = img.Stream,
                        ImageDimension = img.Dimension ?? new()
                    });
            }

            return WithImageFields(imageFields);
        }

        /// <summary>
        /// Adds a table to the template.
        /// </summary>
        /// <param name="placeholder">The placeholder name to be replaced in the document.</param>
        /// <param name="rows">The collection of table rows (<see cref="DocTableRowInputRec"/>).</param>
        /// <param name="tableStyles">Optional table-level styles (e.g., borders, background).</param>
        /// <returns>A new builder instance with the added table.</returns>
        public DocTemplateBuilder WithTable(
            string placeholder,
            IEnumerable<DocTableRowInputRec> rows,
            DocTableStyles? tableStyles = null
        )
        {
            var tableField = new DocTemplateTableField(placeholder, tableStyles)
            {
                Rows = [.. rows.Select((columns, index) =>
            {
                var docColumns = columns.Columns.Select(column => new DocTableCell
                {
                    IsHeader = index == 0,
                    Style = columns.Pipe?.Invoke(column),
                    Value = column
                });

                return new DocTableRow { Columns = [.. docColumns] };
            })],
            };
            return WithTableField(tableField);
        }

        /// <summary>
        /// Adds a pre-built <see cref="DocTemplateTableField"/> to the template.
        /// </summary>
        /// <param name="tableField">The table field to add.</param>
        /// <returns>A new builder instance with the added table field.</returns>
        public DocTemplateBuilder WithTableField(DocTemplateTableField tableField)
        {
            var clone = CloneModel(_docTemplate);
            clone.TableFields.Add(tableField);
            return new DocTemplateBuilder(clone);
        }

        /// <summary>
        /// Finalizes the builder and returns the constructed <see cref="DocTemplateModel"/>.
        /// </summary>
        /// <returns>The constructed <see cref="DocTemplateModel"/> instance.</returns>
        public DocTemplateModel Build()
        {
            return _docTemplate;
        }

        /// <summary>
        /// Helper method to clone a <see cref="DocTemplateModel"/> instance.
        /// Ensures immutability by creating a deep copy before modification.
        /// </summary>
        /// <param name="source">The model to clone.</param>
        /// <returns>A deep copy of the <see cref="DocTemplateModel"/>.</returns>
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
