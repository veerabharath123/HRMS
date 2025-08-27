using HRMS.SharedKernel.Models.Response;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRMS.WebApplication.Class
{
    public class ApiRequest
    {
        private readonly HttpClient _httpClient;

        public ApiRequest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, object? data, bool authRequired)
        {
            var request = new HttpRequestMessage(method, url);
            
            if(data is not null)
            {
                var jsonContent = data is string stringData ? stringData : JsonConvert.SerializeObject(data);
                request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            }

            if(authRequired)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "your_token");
            }

            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));

            return request;
        }

        public Task<ApiResponseDto<TResponse>> PostAsync<TResponse>(string url, bool authRequired, CancellationToken cancellationToken = default)
        {
            return PostAsync<object?, TResponse>(url, null, authRequired, cancellationToken);
        }
        public async Task<ApiResponseDto<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest? data, bool authRequired, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Post, url, data, authRequired);
                var response = await _httpClient.SendAsync(request, cancellationToken);
                return await HandleResponse<TResponse>(response);
            }
            catch (Exception ex)
            {
                // Handle exceptions (log them, rethrow them, etc.)
                return ApiResponseDto<TResponse>.FailureStatus(ex.ToString());
            }
        }
        private async Task<ApiResponseDto<TResponse>> HandleResponse<TResponse>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await HandleSuccessResponse<TResponse>(response);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return ApiResponseDto<TResponse>.FailureStatus(errorMessage);
        }
        private async Task<ApiResponseDto<TResponse>> HandleSuccessResponse<TResponse>(HttpResponseMessage response)
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
                return ApiResponseDto<TResponse>.SuccessStatus(result, "Request successful");
            }
            catch { /* Ignore and fallback to failure below */ }

            return ApiResponseDto<TResponse>.FailureStatus(content);
        }
        private async Task<ApiResponseDto<TResponse>> HandleFailureResponse<TResponse>(HttpResponseMessage response)
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
                return ApiResponseDto<TResponse>.SuccessStatus(result, "Request successful");
            }
            catch { /* Ignore and fallback to failure below */ }

            return ApiResponseDto<TResponse>.FailureStatus(content);
        }
    }
}
