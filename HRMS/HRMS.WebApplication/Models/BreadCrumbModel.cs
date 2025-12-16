namespace HRMS.WebApplication.Models
{
    public class BreadCrumbModel
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public bool IsRoot { get; set; } 
    }
}
