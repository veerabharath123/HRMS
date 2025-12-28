using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HRMS.WebApplication.Helpers
{
    [HtmlTargetElement("input", Attributes = "asp-readonly")]
    public sealed class ReadOnlyTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-readonly")]
        public bool IsReadOnly { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsReadOnly)
            {
                output.Attributes.SetAttribute("readonly", "readonly");
            }
            else
            {
                output.Attributes.RemoveAll("readonly");
            }
        }
    }
}
