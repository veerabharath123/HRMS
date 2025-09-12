using HRMS.Domain.Common;
using System;
using System.Collections.Generic;

namespace HRMS.Application.Common.Class
{
    /// <summary>
    /// Provides extension methods for building advanced tables
    /// in <see cref="DocTemplateBuilder"/> using strongly typed models.
    /// </summary>
    public static class DocTemplateBuilderTableExtension
    {
        /// <summary>
        /// Adds an advanced table to the template using a strongly typed model.
        /// </summary>
        /// <typeparam name="T">The type of data items for the table.</typeparam>
        /// <param name="builder">The current <see cref="DocTemplateBuilder"/> instance.</param>
        /// <param name="advanceTableOptions">
        /// A configuration action that defines columns, styles, and data for the table.
        /// </param>
        /// <returns>A new builder instance with the configured table field.</returns>
        public static DocTemplateBuilder WithTableFromModel<T>(
            this DocTemplateBuilder builder,
            Action<DocTemplateAdvanceTableModel<T>> advanceTableOptions
        )
        {
            var model = new DocTemplateAdvanceTableModel<T>();
            advanceTableOptions.Invoke(model);

            // Use ConvertToTableField extension to transform model into a table field
            var tableField = model.ConvertToTableField();

            return builder.WithTableField(tableField);
        }

        /// <summary>
        /// Configures a table model with a placeholder, data source, and optional styles.
        /// </summary>
        /// <typeparam name="T">The type of the table data.</typeparam>
        /// <param name="model">The table model being configured.</param>
        /// <param name="placeholder">The placeholder text to replace in the template.</param>
        /// <param name="tableData">The collection of data items to populate the table.</param>
        /// <param name="tableStyles">Optional styles applied to the table.</param>
        /// <returns>A list of column definitions to allow further configuration.</returns>
        public static List<DocTemplateAdvanceColumnField<T>> ConfigureTable<T>(
            this DocTemplateAdvanceTableModel<T> model,
            string placeholder,
            IEnumerable<T> tableData,
            DocTableStyles? tableStyles = null
        )
        {
            model.Placeholder = placeholder;
            model.TableData = tableData;
            model.TableStyles = tableStyles;

            return model.TableDefinitions;
        }

        /// <summary>
        /// Adds a column definition to the advanced table model.
        /// </summary>
        /// <typeparam name="T">The type of the table data.</typeparam>
        /// <param name="model">The current list of column definitions.</param>
        /// <param name="columnName">The column name (used in the header row).</param>
        /// <param name="selector">
        /// A function that extracts the value for this column from a data item.
        /// </param>
        /// <param name="pipe">
        /// Optional function to apply custom <see cref="DocTextStyle"/> based on row data.
        /// </param>
        /// <returns>The updated list of column definitions.</returns>
        public static List<DocTemplateAdvanceColumnField<T>> AddColumn<T>(
            this List<DocTemplateAdvanceColumnField<T>> model,
            string columnName,
            Func<T, object?> selector,
            Func<T, DocTextStyle?>? pipeStyles = null
        )
        {
            model.Add(new DocTemplateAdvanceColumnField<T>
            {
                ColumnName = columnName,
                Selector = selector,
                PipeStyles = pipeStyles
            });

            return model;
        }

        /// <summary>
        /// Converts an advanced table model into a <see cref="DocTemplateTableField"/>
        /// by generating header and data rows.
        /// </summary>
        /// <typeparam name="T">The type of the table data.</typeparam>
        /// <param name="model">The table model to convert.</param>
        /// <returns>A <see cref="DocTemplateTableField"/> ready for template replacement.</returns>
        private static DocTemplateTableField ConvertToTableField<T>(
            this DocTemplateAdvanceTableModel<T> model
        )
        {
            var tableField = new DocTemplateTableField
            {
                Name = model.Placeholder,
                Styles = model.TableStyles
            };

            // 1. Header row
            var headerRow = new DocTableRow { Columns = [] };
            foreach (var col in model.TableDefinitions)
            {
                headerRow.Columns.Add(new DocTableCell
                {
                    IsHeader = true,
                    Value = col.ColumnName,
                    Style = new DocTextStyle { Bold = true }
                });
            }
            tableField.Rows.Add(headerRow);

            // 2. Data rows
            foreach (var item in model.TableData)
            {
                var row = new DocTableRow { Columns = [] };

                foreach (var col in model.TableDefinitions)
                {
                    row.Columns.Add(new DocTableCell
                    {
                        Value = col.Selector?.Invoke(item)?.ToString() ?? string.Empty,
                        Style = col.PipeStyles?.Invoke(item),
                    });
                }
                tableField.Rows.Add(row);
            }

            return tableField;
        }
    }

}
