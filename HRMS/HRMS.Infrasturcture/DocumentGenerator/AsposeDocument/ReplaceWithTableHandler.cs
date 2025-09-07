using Aspose.Words;
using Aspose.Words.Replacing;
using Aspose.Words.Tables;
using HRMS.Domain.Common;
using System.Drawing;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
{
    public class ReplaceWithTableHandler : IReplacingCallback
    {
        private readonly DocTemplateTableField _table;
        private readonly DocumentBuilder _builder;

        public ReplaceWithTableHandler(Document doc, DocTemplateTableField table)
        {
            _table = table;
            _builder = new DocumentBuilder(doc);
        }

        public ReplaceAction Replacing(ReplacingArgs e)
        {
            // Move builder to placeholder location
            _builder.MoveTo(e.MatchNode);

            // Create table
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

        private static void ApplyCellStyle(DocumentBuilder builder, DocTableCell cell)
        {
            builder.Font.Bold = cell.IsHeader;

            if (cell.Style != null)
            {
                var font = builder.Font;
                if (!string.IsNullOrWhiteSpace(cell.Style.FontName))
                    font.Name = cell.Style.FontName;
                if (cell.Style.FontSize.HasValue)
                    font.Size = cell.Style.FontSize.Value;
                if (cell.Style.Bold.HasValue)
                    font.Bold = cell.Style.Bold.Value;
                if (cell.Style.Italic.HasValue)
                    font.Italic = cell.Style.Italic.Value;
                if (!string.IsNullOrWhiteSpace(cell.Style.ColorHex))
                    font.Color = ColorTranslator.FromHtml(cell.Style.ColorHex);
            }


        }

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

//public class ReplaceWithTableHandler : IReplacingCallback
//{
//    private readonly List<string[]> _tableData;
//    private readonly DocumentBuilder _builder;

//    public ReplaceWithTableHandler(Document doc, List<string[]> tableData)
//    {
//        _tableData = tableData;
//        _builder = new DocumentBuilder(doc);
//    }

//    public ReplaceAction Replacing(ReplacingArgs e)
//    {
//        // Move builder to the matched text node
//        _builder.MoveTo(e.MatchNode);

//        // Insert the table
//        Table table = _builder.StartTable();
//        foreach (var row in _tableData)
//        {
//            foreach (var cell in row)
//            {
//                _builder.InsertCell();
//                _builder.Write(cell);
//            }
//            _builder.EndRow();
//        }
//        _builder.EndTable();

//        // Remove the placeholder text node
//        e.MatchNode.Remove();

//        return ReplaceAction.Skip;
//    }
//}
