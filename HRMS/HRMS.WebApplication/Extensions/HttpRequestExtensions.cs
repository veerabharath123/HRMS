namespace HRMS.WebApplication.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determines if the request is an AJAX or API request based on common headers
        /// </summary>
        /// <param name="request">The HttpRequest to check</param>
        /// <returns>True if the request is an AJAX or API request, false otherwise</returns>
        public static bool IsAjaxOrApiRequest(this HttpRequest request)
        {
            return request == null
                ? throw new ArgumentNullException(nameof(request))
                : IsXmlHttpRequest(request) ||
                   IsJsonRequest(request) ||
                   HasApiHeaders(request);
        }

        private static bool IsXmlHttpRequest(HttpRequest request) =>
            request.Headers.XRequestedWith == "XMLHttpRequest";

        private static bool IsJsonRequest(HttpRequest request) =>
            request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase);

        private static bool HasApiHeaders(HttpRequest request) =>
            request.Headers.ContainsKey("Authorization") ||
            request.Headers.ContainsKey("X-API-KEY");
    }
}
