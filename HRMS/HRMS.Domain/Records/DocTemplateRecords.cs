using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Records
{
    public static class DocTemplateRecords
    {
        public record DocImageInfoRec(string Placeholder, string? Path = null, Stream? Stream = null, ImageDimension Dimension = null!);
        public record DocTableRowInputRec(
            string[] Columns,
            Func<string, DocTextStyle?>? Pipe = null
        );
    }
}
