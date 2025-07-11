using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkWithAyodeji.Service.Dto.Response
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponseDto(bool success, string message, T data, List<string> errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        public static ApiResponseDto<T> SuccessResponse(string message, T data)
        {
            return new ApiResponseDto<T>(true, message, data);
        }

        public static ApiResponseDto<T> ErrorResponse(string message,T data, List<string> errors = null)
        {
            return new ApiResponseDto<T>(false, message, data, errors);
        }

    }
}
