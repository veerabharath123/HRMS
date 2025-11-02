using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class ApiResponseDto
    {
        public int StatusCode { get; set; }
        public dynamic? Result { get; set; }
        public bool HasResult { 
            get => Result is not null;
        }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool Logout { get; set; }

        internal ApiResponseDto(int statusCode, object? result, string message, bool success)
        {
            StatusCode = statusCode;
            Result = result;
            Message = message;
            Success = success;
        }
        public ApiResponseDto() { }
        public static ApiResponseDto SuccessStatus(object? result, string message = "")
        {
            return new ApiResponseDto(200, result, message, true);
        }
        public static ApiResponseDto FailureStatus(string message = "")
        {
            return new ApiResponseDto(200, default, message, false);
        }
        public static ApiResponseDto FailureStatus(string message, params string[] messageParams)
        {
            return new ApiResponseDto(200, default, string.Format(message, messageParams), false);
        }
        public static ApiResponseDto FlagStatus(bool success, string message = "")
        {
            return new ApiResponseDto(200, success, message, success);
        }
        public static ApiResponseDto CustomStatus(int statusCode, bool success, object? result, string message)
        {
            return new ApiResponseDto(statusCode, result, message, success);
        }
    }
}
