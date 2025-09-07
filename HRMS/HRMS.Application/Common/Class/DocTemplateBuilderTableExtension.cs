using HRMS.Domain.Common;
using System;
using System.Collections.Generic;

namespace HRMS.Application.Common.Class
{
    public static class DocTemplateBuilderTableExtension
    {
        public static DocTemplateBuilder WithTable<T>(
            this DocTemplateBuilder builder,
            Action<DocTemplateAdvanceTableModel<T>> advanceTableOptions
        )
        {
            var model = new DocTemplateAdvanceTableModel<T>();
            advanceTableOptions.Invoke(model);

            // use your existing ConvertToTableField extension
            var tableField = model.ConvertToTableField();

            return builder.WithTableField(tableField);
        }
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
        public static List<DocTemplateAdvanceColumnField<T>> AddColumn<T>(
            this List<DocTemplateAdvanceColumnField<T>> model,
            string columnName,
            Func<T, object?> selector,
            Func<T, DocTextStyle?>? pipe = null
        )
        {
            model.Add(new DocTemplateAdvanceColumnField<T>
            {
                ColumnName = columnName,
                Selector = selector,
                PipeStyles = pipe
            });

            return model;
        }
        private static DocTemplateTableField ConvertToTableField<T>(
            this DocTemplateAdvanceTableModel<T> model
        )
        {
            var tableField = new DocTemplateTableField { Name = model.Placeholder, Styles = model.TableStyles };

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
                        Style = col.PipeStyles?.Invoke(item)
                    });
                }
                tableField.Rows.Add(row);
            }

            return tableField;
        }

    }
}
