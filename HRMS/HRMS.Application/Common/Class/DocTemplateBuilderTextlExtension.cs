using HRMS.Domain.Common;
using HRMS.SharedKernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Class
{
    public static class DocTemplateBuilderTextlExtension
    {
        public static DocTemplateBuilder WithTextFromModel<T>(
            this DocTemplateBuilder builder,
            T model
        )
        {
            var textFields = LoadFromModel(model);

            return builder.WithTextFields(textFields);
        }
        private static IEnumerable<DocTemplateTextField> LoadFromModel<T>(T model)
        {
            List<DocTemplateTextField> textFields = new();   

            foreach (var prop in typeof(T).GetProperties())
            {
                // Get key from Placeholder or DisplayName
                var placeholderAttr = prop.GetCustomAttribute<DocumentPlaceholderAttribute>();

                if (placeholderAttr == null) continue;

                string key = placeholderAttr.Key ?? prop.Name;

                object? rawValue = prop.GetValue(model);

                if (rawValue == null) continue;

                textFields.Add(new DocTemplateTextField
                {
                    Name = key,
                    Value = rawValue.ToString()
                });
            }

            return textFields;
        }
        public static void LoadImages(this DocTemplateModel docModel,string placeholder, string imagePath, ImageDimension? dimension = null)
        {
            var imageField = new DocTemplateImageField
            {
                Name = placeholder,
                ImagePath = imagePath,
                ImageDimension = dimension ?? new()
            };

            docModel.ImageFields.Add(imageField);
        }
        public static void LoadImages(this DocTemplateModel docModel, string placeholder, Stream imageStream, ImageDimension dimension)
        {
            var imageField = new DocTemplateImageField
            {
                Name = placeholder,
                ImageStream = imageStream,
                ImageDimension = dimension
            };

            docModel.ImageFields.Add(imageField);
        }
    }
}
