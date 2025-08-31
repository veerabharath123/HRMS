using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class ApiResponseDto<T>
    {
        public int StatusCode { get; set; }
        public T? Result { get; set; }
        public bool HasResult { 
            get => Result is not null;
        }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool Logout { get; set; }

        internal ApiResponseDto(int statusCode, T? result, string message, bool success)
        {
            StatusCode = statusCode;
            Result = result;
            Message = message;
            Success = success;
        }

        public static ApiResponseDto<T> SuccessStatus(T? result, string message = "")
        {
            return new ApiResponseDto<T>(200, result, message, true);
        }
        public static ApiResponseDto<T> FailureStatus(string message = "")
        {
            return new ApiResponseDto<T>(200, default, message, false);
        }
        public static ApiResponseDto<T> FailureStatus(string message, params string[] messageParams)
        {
            return new ApiResponseDto<T>(200, default, string.Format(message, messageParams), false);
        }
        public static ApiResponseDto<T> CustomStatus(int statusCode, bool success, T? result, string message)
        {
            return new ApiResponseDto<T>(statusCode, result, message, success);
        }
    }
}
