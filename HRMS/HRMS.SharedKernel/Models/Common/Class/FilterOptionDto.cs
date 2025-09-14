using HRMS.SharedKernel.Models.Common.Enum;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class FilterOptionDto
    {
        public string PropertyName { get; set; } = string.Empty;
        public FilterPropertyType PropertyType { get; set; } = FilterPropertyType.String;
        public object? Value { get; set; }
        public FilterComparator Comparator { get; set; }
        public FilterOperator Operator { get; set; } = FilterOperator.And;
    }
}
