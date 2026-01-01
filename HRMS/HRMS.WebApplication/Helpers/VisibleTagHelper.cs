using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HRMS.WebApplication.Helpers
{
    [HtmlTargetElement(Attributes = "asp-visible")]
    public sealed class VisibleTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-visible")]
        public bool IsVisible { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("asp-d-none");

            if (IsVisible)
            {
                return;
            }

            if (output.Attributes.TryGetAttribute("class", out var classAttr))
            {
                var existing = classAttr.Value?.ToString() ?? string.Empty;

                if (!existing.Contains("d-none"))
                {
                    output.Attributes.SetAttribute(
                        "class",
                        $"{existing} d-none".Trim()
                    );
                }
            }
            else
            {
                output.Attributes.SetAttribute("class", "d-none");
            }            
        }
    }
}
