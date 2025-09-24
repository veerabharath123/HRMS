using HRMS.SharedKernel.Models.Response;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRMS.WebApplication.Class
{
    public class ApiRequest
    {
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiRequest(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _apiBaseUrl = configuration["WebAppSettings:ApiBaseUrl"] ?? throw new ArgumentNullException("API Base URL is not configured.");
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, Uri url, object? data, bool authRequired)
        {
            var request = new HttpRequestMessage(method, url);
            
            if(data is not null)
            {
                var jsonContent = data is string stringData ? stringData : JsonConvert.SerializeObject(data);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }

            if(authRequired)
            {
                var token = _httpContextAccessor?.HttpContext?.Session.GetString("AccessToken");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token ?? string.Empty);
            }

            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));

            return request;
        }
        private static bool TryCreateUri(string url, out Uri? uri)
        {
            uri = null;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return false;

            uri = new Uri(url);

            return uri.Scheme.Equals(Uri.UriSchemeHttps);
        }

        public Task<ApiResponseDto<TResponse>> PostAsync<TResponse>(string actionPath, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            if(!TryCreateUri(_apiBaseUrl + actionPath, out Uri? uri))
                return Task.FromResult(ApiResponseDto<TResponse>.FailureStatus("Invalid URL"));

            return PostAsync<object?, TResponse>(uri!, null, authRequired, cancellationToken);
        }
        public Task<ApiResponseDto<TResponse>> PostAsync<TResponse>(string actionPath, object? data, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            if (!TryCreateUri(_apiBaseUrl + actionPath, out Uri? uri))
                return Task.FromResult(ApiResponseDto<TResponse>.FailureStatus("Invalid URL"));

            return PostAsync<object?, TResponse>(uri!, data, authRequired, cancellationToken);
        }
        public Task<ApiResponseDto<TResponse>> PostAsync<TRequest, TResponse>(string actionPath, TRequest? data, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            if (!TryCreateUri(_apiBaseUrl + actionPath, out Uri? uri))
                return Task.FromResult(ApiResponseDto<TResponse>.FailureStatus("Invalid URL"));

            return PostAsync<TRequest, TResponse>(uri!, data, authRequired, cancellationToken);
        }
        private async Task<ApiResponseDto<TResponse>> PostAsync<TRequest, TResponse>(Uri url, TRequest? data, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                using var request = CreateRequest(HttpMethod.Post, url, data, authRequired);
                
                using var response = await httpClient.SendAsync(request, cancellationToken);
                return await HandleResponse<TResponse>(response);
            }
            catch (WebException ex)
            {
                // Handle exceptions (log them, rethrow them, etc.)
                return ApiResponseDto<TResponse>.FailureStatus(ex.ToString());
            }
        }
        private async Task<ApiResponseDto<TResponse>> HandleResponse<TResponse>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            // Try to deserialize as ApiResponseDto<TResponse>
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<TResponse>>(content);
                if (apiResponse != null)
                    return apiResponse;
            }
            catch { /* Ignore and try next */ }

            // Try to deserialize as TResponse
            try
            {
                var result = JsonConvert.DeserializeObject<TResponse>(content);
                if (response.IsSuccessStatusCode)
                    return ApiResponseDto<TResponse>.SuccessStatus(result, "Request successful");
                else
                    return ApiResponseDto<TResponse>.FailureStatus("Request Failed");
            }
            catch { return ApiResponseDto<TResponse>.FailureStatus(content); }
        }
    }
}
