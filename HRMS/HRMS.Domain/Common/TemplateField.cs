using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Common
{
    public class DocTemplateTextField
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }

        // Optional styling for text
        public string? FontName { get; set; }
        public double? FontSize { get; set; }
        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public Color? Color { get; set; }
    }
    public class DocTemplateImageField
    {
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public Stream? ImageStream { get; set; }
        public ImageDimension ImageDimension { get; set; }
        public double? Height { get; set; }
        public DocTemplateImageField(string name, string? imagePath, double? width = default, double? height = default)
        {
            Name = name;
            ImagePath = imagePath;
            ImageDimension = new ImageDimension(width, height);
        }
        public DocTemplateImageField(string name, Stream? imageStream, double? width = default, double? height = default)
        {
            Name = name;
            ImageStream = imageStream;
            ImageDimension = new ImageDimension(width, height);
        }
        public DocTemplateImageField()
        {
            ImageDimension = new();
        }
    }
    public class DocTemplateModel
    {
        public string StartIndicator { get; set; } = "<";
        public string EndIndicator { get; set; } = ">";
        public List<DocTemplateTextField> TextFields { get; set; } = [];
        public List<DocTemplateImageField> ImageFields { get; set; } = [];
        public DocTemplateModel(string startIndicator, string endIndicator)
        {
            StartIndicator = startIndicator;
            EndIndicator = endIndicator;
        }
        public DocTemplateModel() { }
        
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
}
