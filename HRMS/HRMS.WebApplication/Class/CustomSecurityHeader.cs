using System.Security.Cryptography;

namespace HRMS.WebApplication.Class
{
    public class CustomSecurityHeader
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public CustomSecurityHeader(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            string AppBaseUrl = _configuration["MyConfig:SiteUrl"] ?? string.Empty;

            var rng = RandomNumberGenerator.Create();
            var nonceBytes = new byte[32];

            rng.GetBytes(nonceBytes);
            var nonce = Convert.ToBase64String(nonceBytes);

            string CSP = string.Format("default-src 'self'; " +
                    "connect-src 'self' https://maps.googleapis.com https://static.zdassets.com/ https://ekr.zdassets.com/ https://curacaotourist.zendesk.com/ https://*.googleapis.com https://*.google.com wss://widget-mediator.zopim.com wss://localhost:44346/ ;" +
                    "script-src 'self' https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/ https://maps.googleapis.com https://maps.gstatic.com https://static.zdassets.com 'nonce-{0}'; " +
                    "style-src 'self' https://maps.googleapis.com https://maps.gstatic.com https://fonts.googleapis.com 'nonce-{0}' 'strict-dynamic'; " +
                    "style-src-elem 'self' https://maps.googleapis.com https://maps.gstatic.com https://fonts.googleapis.com 'nonce-{0}' 'strict-dynamic'; " +
                    "img-src 'self' https://maps.gstatic.com/ data: https://maps.gstatic.com; " +
                    "font-src 'self' https://fonts.gstatic.com;" +
                    "form-action 'self';" +
                    "media-src 'self' https://static.zdassets.com/;" +
                    "frame-src 'self' https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/;" +
                    "frame-ancestors 'none'; " +
                    "block-all-mixed-content;",
                nonce);

            context = SetHeaders(context, nonce, CSP);
            context.Items["ScriptNonce"] = nonce;

            context.Response.OnStarting(() =>
            {
                // Remove the Server header from the HTTP response for all requests
                context.Response.Headers.Remove("server");
                context.Response.Headers.Remove("Sec-CH-UA-Platform");

                return Task.CompletedTask;
            });

            await _next.Invoke(context);
        }

        private HttpContext SetHeaders(HttpContext context, string nonce, string CSP)
        {
            context.Response.Headers.Append("ScriptNonce", nonce);

            context.Response.Headers.Append("X-Frame-Options", "DENY");

            context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

            context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");

            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            context.Response.Headers.Append("Permissions-Policy", "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");

            context.Response.Headers.Append("Content-Security-Policy", CSP);

            return context;
        }
    }
}
