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
    /// <summary>
    /// Extension methods for <see cref="DocTemplateBuilder"/> to add text fields
    /// directly from strongly-typed model classes decorated with
    /// <see cref="DocumentPlaceholderAttribute"/>.
    /// </summary>
    public static class DocTemplateBuilderTextExtension
    {
        /// <summary>
        /// Adds multiple text fields to the template by reading property values
        /// from a model. Only properties decorated with
        /// <see cref="DocumentPlaceholderAttribute"/> are considered.
        /// </summary>
        /// <typeparam name="T">The type of the model to extract placeholders from.</typeparam>
        /// <param name="builder">The <see cref="DocTemplateBuilder"/> instance being extended.</param>
        /// <param name="model">The model instance whose properties provide text field values.</param>
        /// <returns>
        /// A new <see cref="DocTemplateBuilder"/> instance containing all generated
        /// <see cref="DocTemplateTextField"/>s.
        /// </returns>
        public static DocTemplateBuilder WithTextFromModel<T>(
            this DocTemplateBuilder builder,
            T model
        )
        {
            var textFields = LoadFromModel(model);

            return builder.WithTextFields(textFields);
        }

        /// <summary>
        /// Extracts <see cref="DocTemplateTextField"/> entries from a model instance.
        /// Only properties decorated with <see cref="DocumentPlaceholderAttribute"/> are processed.
        /// The property name or the attribute's <c>Key</c> is used as the placeholder.
        /// </summary>
        /// <typeparam name="T">The type of the model to inspect.</typeparam>
        /// <param name="model">The model instance to extract values from.</param>
        /// <returns>
        /// A list of <see cref="DocTemplateTextField"/> containing name-value pairs
        /// corresponding to placeholders and their resolved values.
        /// </returns>
        private static List<DocTemplateTextField> LoadFromModel<T>(T model)
        {
            List<DocTemplateTextField> textFields = new();

            foreach (var prop in typeof(T).GetProperties())
            {
                // Get key from Placeholder attribute
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
    }

}
