using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Common
{
    public class DocTextStyle
    {
        public string? FontName { get; set; }
        public double? FontSize { get; set; }
        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public string? Color { get; set; }
    }
    public class DocTemplateTextField
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
        public DocTextStyle? Style { get; set; }
    }
    public class DocTemplateImageField
    {
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public Stream? ImageStream { get; set; }
        public ImageFieldDimension ImageDimension { get; set; } = new();
    }
    public class DocTemplateModel
    {
        public string StartIndicator { get; set; } = string.Empty;
        public string EndIndicator { get; set; } = string.Empty;

        public List<DocTemplateTextField> TextFields { get; set; } = [];
        public List<DocTemplateImageField> ImageFields { get; set; } = [];
        public List<DocTemplateTableField> TableFields { get; set; } = [];

        public DocTemplateModel() { }
        public DocTemplateModel(string startIndicator, string endIndicator)
        {
            StartIndicator = startIndicator;
            EndIndicator = endIndicator;
        }
    }
    public class ImageFieldDimension
    {
        public double? Width { get; set; }
        public double? Height { get; set; }
        public ImageFieldDimension(double? width, double? height)
        {
            Width = width;
            Height = height;
        }
        public ImageFieldDimension()
        {
            
        }
    }
    public class DocTemplateAdvanceColumnField<T>
    {
        public string ColumnName { get; set; } = string.Empty; 
        public Func<T, object?>? Selector { get; set; }
        public Func<T, DocTextStyle?>? PipeStyles { get; set; }

        public DocTemplateAdvanceColumnField(Func<T, object?> selector, string columnName, Func<T, DocTextStyle?>? pipeStyles = null)
        {
            Selector = selector;
            ColumnName = columnName;
            PipeStyles = pipeStyles;
        }
        public DocTemplateAdvanceColumnField()
        {
            
        }
    }
    public class DocTemplateAdvanceTableModel<T>
    {
        public string Placeholder { get; set; } = string.Empty;
        public List<DocTemplateAdvanceColumnField<T>> TableDefinitions { get; set; } = [];
        public IEnumerable<T> TableData { get; set; } = [];
        public DocTableStyles? TableStyles { get; set; }

        public DocTemplateAdvanceTableModel(string placeholder, IEnumerable<T> tableData, DocTableStyles? tableStyles = null)
        {
            Placeholder = placeholder;
            TableData = tableData;
            TableStyles = tableStyles;
        }
        public DocTemplateAdvanceTableModel()
        {
            
        }
    }

    public class DocTemplateTableField
    {
        public string Name { get; set; } = string.Empty; 
        public List<DocTableRow> Rows { get; set; } = []; 

        public DocTableStyles? Styles { get; set; }
        public DocTemplateTableField(string name, DocTableStyles? styles = null)
        {
            Name = name;
            Styles = styles;
        }
        public DocTemplateTableField()
        {
            
        }
    }
    public class DocTableRow
    {
        public List<DocTableCell> Columns { get; set; } = [];
    }
    public class DocTableCell
    {
        public bool IsHeader { get; set; } = false;
        public DocTextStyle? Style { get; set; }
        public string Value { get; set; } = string.Empty;
        
    }
    public class DocTableStyles
    {
        public double? BorderWidth { get; set; }
        public string? BorderStyle { get; set; }
        public string? BorderColorHex { get; set; }
        public string? VerticalAlignment { get; set; }
        public string? HorizontalAlignment { get; set; }
    }
}
