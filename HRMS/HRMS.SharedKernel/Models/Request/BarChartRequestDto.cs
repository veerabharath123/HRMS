using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class BarChartRequestDto
    {
        public BarCharBasicData? BasicData { get; set; }
        public string Message { get; set; } = string.Empty;

    }
    public class BarCharBasicData
    {
        public List<string> Labels { get; set; } = [];
        public List<BarChartDataSet> Datasets { get; set; } = [];
    }
    public class BarChartDataSet
    {
        public string Label { get; set; } = string.Empty;
        public List<object> Data { get; set; } = [];

        public List<string> BackgroundColor { get; set; } = [];
        public List<string> BorderColor { get; set; } = [];
        public int BorderWidth { get; set; } = 1;
    }

}
