using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.WebApplication.Models
{
    public class BaseRefModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public static SelectList ToSelectList(IEnumerable<BaseRefModel> items)
        {
            return new SelectList(items, nameof(Id), nameof(Name));
        }
    }
}
