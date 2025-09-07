using Aspose.Words;
using Aspose.Words.Replacing;
using Aspose.Words.Tables;
using HRMS.Domain.Common;
using System.Drawing;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
{
    /// <summary>
    /// Implements <see cref="IReplacingCallback"/> to replace a placeholder in a Word document
    /// with a table. Supports per-cell styling and optional global table styles.
    /// </summary>
    public class ReplaceWithTableHandler : IReplacingCallback
    {
        private readonly DocTemplateTableField _table;
        private readonly DocumentBuilder _builder;

        /// <summary>
        /// Initializes a new instance of <see cref="ReplaceWithTableHandler"/>.
        /// </summary>
        /// <param name="doc">The Aspose <see cref="Document"/> where the table will be inserted.</param>
        /// <param name="table">The table data and styles (<see cref="DocTemplateTableField"/>).</param>
        public ReplaceWithTableHandler(Document doc, DocTemplateTableField table)
        {
            _table = table;
            _builder = new DocumentBuilder(doc);
        }

        /// <summary>
        /// Replaces the matched placeholder with the table content.
        /// </summary>
        /// <param name="e">The replacement arguments containing the matched node.</param>
        /// <returns>A <see cref="ReplaceAction"/> indicating to skip further replacement.</returns>
        public ReplaceAction Replacing(ReplacingArgs e)
        {
            // Move builder to placeholder location
            _builder.MoveTo(e.MatchNode);

            // Start table
            Table table = _builder.StartTable();

            foreach (var row in _table.Rows)
            {
                foreach (var cell in row.Columns)
                {
                    _builder.InsertCell();

                    // Apply cell style if provided
                    ApplyCellStyle(_builder, cell);

                    // Write cell text
                    _builder.Write(cell.Value);
                }
                _builder.EndRow();
            }

            _builder.EndTable();

            // Apply global table styles (optional)
            ApplyTableStyles(table, _table.Styles);

            // Remove placeholder text
            e.MatchNode.Remove();

            return ReplaceAction.Skip;
        }

        /// <summary>
        /// Applies styling to a table cell. Automatically applies header bolding and optional font settings.
        /// </summary>
        /// <param name="builder">The <see cref="DocumentBuilder"/> used to write the cell.</param>
        /// <param name="cell">The cell data and style.</param>
        private static void ApplyCellStyle(DocumentBuilder builder, DocTableCell cell)
        {
            // Apply header bold automatically
            builder.Font.Bold = cell.IsHeader || (cell.Style?.Bold ?? false);

            // Apply optional style properties using null-coalescing pattern
            if (cell.Style == null) return;

            var font = builder.Font;

            font.Name = string.IsNullOrWhiteSpace(cell.Style.FontName) ? font.Name : cell.Style.FontName;
            font.Size = cell.Style.FontSize ?? font.Size;
            font.Italic = cell.Style.Italic ?? font.Italic;

            // Only set color if a valid hex is provided
            if (!string.IsNullOrWhiteSpace(cell.Style.ColorHex))
            {
                try
                {
                    font.Color = ColorTranslator.FromHtml(cell.Style.ColorHex);
                }
                catch
                {
                    // Ignore invalid color and leave existing font color
                }
            }
        }


        /// <summary>
        /// Applies optional global table styles such as borders and alignment.
        /// </summary>
        /// <param name="table">The Aspose <see cref="Table"/> object to style.</param>
        /// <param name="styles">Optional <see cref="DocTableStyles"/> containing style settings.</param>
        private static void ApplyTableStyles(Table table, DocTableStyles? styles)
        {
            if (styles == null) return;

            if (styles.BorderWidth.HasValue)
            {
                var bw = styles.BorderWidth.Value;
                var bc = !string.IsNullOrWhiteSpace(styles.BorderColorHex)
                    ? ColorTranslator.FromHtml(styles.BorderColorHex)
                    : Color.Black;

                // Set all borders (top/bottom/left/right/inside) to the same style
                table.SetBorders(LineStyle.Single, bw, bc);
            }

            if (!string.IsNullOrWhiteSpace(styles.HorizontalAlignment))
            {
                table.Alignment = styles.HorizontalAlignment.ToLower() switch
                {
                    "center" => TableAlignment.Center,
                    "right" => TableAlignment.Right,
                    _ => TableAlignment.Left
                };
            }
        }
    }

}

