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
        //private webresult ReturnWebExceptionStatusProtocolError(HttpWebResponse? myHttpWebResponse)
        //{
        //    _logger.LogError(GeneralConstants.PortocalError, myHttpWebResponse?.StatusCode.ToString(), myHttpWebResponse?.ToString());

        //    var statuscode = myHttpWebResponse?.StatusCode ?? HttpStatusCode.BadRequest;
        //    string code = statuscode.ToString();


        //    return statuscode switch
        //    {
        //        HttpStatusCode.BadRequest => new webresult() { resstring = string.Empty, statuscode = code, error = exmessage, message = exInnerException },
        //        HttpStatusCode.Unauthorized => new webresult() { resstring = string.Empty, statuscode = code, error = exmessage, message = exInnerException, login = true },
        //        HttpStatusCode.Forbidden => new webresult() { resstring = string.Empty, statuscode = code, error = exmessage, message = exInnerException },
        //        HttpStatusCode.NotFound => new webresult() { resstring = string.Empty, statuscode = code, error = exmessage, message = exInnerException },
        //        HttpStatusCode.LengthRequired => new webresult() { resstring = string.Empty, statuscode = code, error = exmessage, message = exInnerException },
        //        HttpStatusCode.TooManyRequests => new webresult() { resstring = string.Empty, statuscode = code, error = exmessage, message = exInnerException },
        //        HttpStatusCode.InternalServerError => new webresult() { resstring = string.Empty, statuscode = code, error = exmessage, message = exInnerException },
        //        _ => new webresult() { resstring = string.Empty, statuscode = code },
        //    };
        //}


        //private webresult ReturnException(WebException ex)
        //{
        //    var myHttpWebResponse = (HttpWebResponse?)ex.Response;
        //    _logger.LogError(GeneralConstants.WebExceptionError, ex.ToString());
        //    return new webresult() { resstring = string.Empty, statuscode = myHttpWebResponse?.StatusCode.ToString() ?? string.Empty, message = exInnerException, error = exmessage };
        //}

        //private webresult HandleAndReturnWebException(WebException ex)
        //{
        //    exmessage = ex.Message;
        //    exInnerException = ex.InnerException?.ToString() ?? string.Empty;

        //    if (ex.Status == WebExceptionStatus.ProtocolError)
        //        return ReturnWebExceptionStatusProtocolError((HttpWebResponse?)ex.Response);

        //    return ReturnException(ex);
        //}

        //protected async Task<webresult> ApiCall(string apiMethod, string jsonParams, bool deserialize = false, bool needAuth = false)
        //{
        //    if (!TryValidateAndCreateUriString(apiMethod, out Uri? uri))
        //        return new webresult() { resstring = string.Empty, statuscode = StatusCodes.Status400BadRequest.ToString() };
        //    else
        //    {
        //        try
        //        {
        //            using HttpClient httpClient = CreateHttpClient();
        //            LoadClientSettings(httpClient, needAuth);
        //            HttpResponseMessage response = await httpClient.PostAsync(uri, new StringContent(jsonParams, Encoding.UTF8, "application/json"));

        //            return await ProcessResponseAndReturnResultAsync(response, deserialize);
        //        }
        //        catch (WebException ex)
        //        {
        //            return HandleAndReturnWebException(ex);
        //        }
        //    }
        //}
    }
}
