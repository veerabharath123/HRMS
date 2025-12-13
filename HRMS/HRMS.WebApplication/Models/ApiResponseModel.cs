using HRMS.SharedKernel.Models.Response;

namespace HRMS.WebApplication.Models
{
    public class ApiResponseModel<T>
    {
        public int StatusCode { get; set; }
        public T? Result { get; set; }
        public bool HasResult
        {
            get => Result is not null;
        }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool Logout { get; set; }

        public ApiResponseModel()
        {
            
        }
        internal ApiResponseModel(int statusCode, T? result, string message, bool success)
        {
            StatusCode = statusCode;
            Result = result;
            Message = message;
            Success = success;
        }

        public static ApiResponseModel<T> SuccessStatus(T? result, string message = "")
        {
            return new ApiResponseModel<T>(200, result, message, true);
        }
        public static ApiResponseModel<T> FailureStatus(string message = "")
        {
            return new ApiResponseModel<T>(200, default, message, false);
        }
        public static ApiResponseModel<T> UnauthorizedStatus(string message = "")
        {
            return new ApiResponseModel<T> { Success = false, StatusCode = 401, Message = message, Logout = true };
        }
        public static ApiResponseModel<T> ForbiddenStatus(string message = "")
        {
            return new ApiResponseModel<T> { Success = false, StatusCode = 403, Message = message };
        }
    }
}
