using HRMS.WebApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS.WebApplication.Class
{
    public class ApiRequest
    {
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        private const string APIBASE_URL_SETTING = "WebAppSettings:ApiBaseUrl";
        private const string API_REQUEST_FAILED = "Request failed.";
        private const string API_AUTH_NAME = "Bearer";
        private const string NOT_A_VALID_URL = "Invalid Url.";

        public ApiRequest(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _apiBaseUrl = configuration[APIBASE_URL_SETTING] ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }
        private string GetAccessToken()
        {
            var token = _httpContextAccessor?.HttpContext?.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
            {
                token = _httpContextAccessor?.HttpContext?.User.Claims
                    .FirstOrDefault(c => c.Type == "AccessToken")?.Value?.ToString()
                    ?? string.Empty;
            }
            return token ?? string.Empty;
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
                request.Headers.Authorization = new AuthenticationHeaderValue(API_AUTH_NAME, GetAccessToken());
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
        public Task<ApiResponseModel<object>> PostAsync(string actionPath, object? data, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            return PostAsync<object?, object>(actionPath, data, authRequired, cancellationToken);
        }
        public Task<ApiResponseModel<TResponse>> PostAsync<TResponse>(string actionPath, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            if(!TryCreateUri(_apiBaseUrl + actionPath, out Uri? uri))
                return Task.FromResult(ApiResponseModel<TResponse>.FailureStatus(NOT_A_VALID_URL));

            return PostAsync<object?, TResponse>(uri!, null, authRequired, cancellationToken);
        }
        public Task<ApiResponseModel<TResponse>> PostAsync<TResponse>(string actionPath, object? data, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            if (!TryCreateUri(_apiBaseUrl + actionPath, out Uri? uri))
                return Task.FromResult(ApiResponseModel<TResponse>.FailureStatus(NOT_A_VALID_URL));

            return PostAsync<object?, TResponse>(uri!, data, authRequired, cancellationToken);
        }
        public Task<ApiResponseModel<TResponse>> PostAsync<TRequest, TResponse>(string actionPath, TRequest? data, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            if (!TryCreateUri(_apiBaseUrl + actionPath, out Uri? uri))
                return Task.FromResult(ApiResponseModel<TResponse>.FailureStatus(NOT_A_VALID_URL));

            return PostAsync<TRequest, TResponse>(uri!, data, authRequired, cancellationToken);
        }
        public async Task<SelectList> DropdownListAsync(string action, object? data = null, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            var response = await PostAsync<List<BaseRefModel>>(action, data, authRequired, cancellationToken);
            var dropdownList = response.Success && response.HasResult ? response.Result : [];

            return BaseRefModel.ToSelectList(dropdownList!);
        }
        private async Task<ApiResponseModel<TResponse>> PostAsync<TRequest, TResponse>(Uri url, TRequest? data, bool authRequired = false, CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                using var request = CreateRequest(HttpMethod.Post, url, data, authRequired);
                
                using var response = await httpClient.SendAsync(request, cancellationToken);
                return await HandleResponse<TResponse>(response);
            }
            catch (WebException ex)
            {
                // Handle exceptions (log them, rethrow them, etc.)
                return ApiResponseModel<TResponse>.FailureStatus(ex.ToString());
            }
        }
        public async Task<RawFileResponse?> GetRawFileAsync(
            string actionPath,
            bool authRequired = false,
            CancellationToken cancellationToken = default)
        {
            if (!TryCreateUri(_apiBaseUrl + actionPath, out var uri))
                return null;

            var request = CreateRequest(HttpMethod.Get, uri!, null, authRequired);

            var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                response.Dispose();
                return null;
            }

            return new RawFileResponse(response);
        }
        private static async Task<ApiResponseModel<TResponse>> HandleResponse<TResponse>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResponseModel<TResponse>.UnauthorizedStatus(
                    "Your session has expired. Please log in again."
                );
            }

            // 🚫 Forbidden → no access
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return ApiResponseModel<TResponse>.ForbiddenStatus(
                    "You do not have permission to access this resource."
                );
            }

            // Try to deserialize as ApiResponseModel<TResponse>
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(content);
                if (apiResponse != null)
                    return apiResponse;
            }
            catch { /* Ignore and try next */ }

            // Try to deserialize as TResponse
            try
            {
                var result = JsonConvert.DeserializeObject<TResponse>(content);
                if (response.IsSuccessStatusCode)
                    return ApiResponseModel<TResponse>.SuccessStatus(result);
                else
                    return ApiResponseModel<TResponse>.FailureStatus(API_REQUEST_FAILED);
            }
            catch { return ApiResponseModel<TResponse>.FailureStatus(content); }
        }
    }
}
