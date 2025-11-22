using HRMS.SharedKernel.Models.Common.Enum;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class FilterOptionDto 
    {
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;// = FilterPropertyType.String;
        public string? Value { get; set; }
        public string Comparator { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;// = FilterOperator.And;
    }
}
