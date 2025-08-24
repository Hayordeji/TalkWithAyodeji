using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkWithAyodeji.Service.Dto.Response
{
    public class ServiceResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string Errors { get; set; }

        public ServiceResponseDto(bool success, string message, T data, string errors = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? errors;
        }

        public static ServiceResponseDto<T> SuccessResponse(string message, T data)
        {
            return new ServiceResponseDto<T>(true, message, data);
        }

        public static ServiceResponseDto<T> ErrorResponse(string message,T data, string errors = null)
        {
            return new ServiceResponseDto<T>(false, message, data, errors);
        }

    }
}
