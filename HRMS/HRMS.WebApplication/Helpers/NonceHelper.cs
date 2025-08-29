using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.WebApplication.Helpers
{
    public static class NonceHelper
    {
        public static IHtmlContent ScriptNonce(this IHtmlHelper htmlHelper)
        {
            var nonce = htmlHelper.ViewContext.HttpContext.Response.Headers["ScriptNonce"];
            return new HtmlString(nonce);
        }
    }
}
