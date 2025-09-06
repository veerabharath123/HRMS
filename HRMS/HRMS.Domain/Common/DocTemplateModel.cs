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
        public string? ColorHex { get; set; }
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
        public ImageDimension ImageDimension { get; set; } = new();
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
    public class ImageDimension
    {
        public double? Width { get; set; }
        public double? Height { get; set; }
        public ImageDimension(double? width, double? height)
        {
            Width = width;
            Height = height;
        }
        public ImageDimension()
        {
            
        }
    }
    public class DocTemplateTableField
    {
        public string Name { get; set; } = string.Empty; // placeholder key
        public List<string[]> Rows { get; set; } = []; // each array = a row
        public int[]? ColumnWidths { get; set; } = null; // optional column widths
    }
}
