namespace HRMS.WebApplication.Class
{
    public sealed class RawFileResponse : IDisposable
    {
        public HttpResponseMessage Response { get; }

        public Stream Stream => Response.Content.ReadAsStream();

        public string ContentType =>
            Response.Content.Headers.ContentType?.ToString()
            ?? "application/octet-stream";

        public RawFileResponse(HttpResponseMessage response)
        {
            Response = response;
        }

        public void Dispose()
        {
            Response.Dispose();
        }
    }

}
