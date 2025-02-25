using System.Collections.Generic;

namespace IATec.Shared.HttpClient.Dto
{
    public class ResponseDto<T>
    {
        internal ResponseDto(bool success)
        {
            Success = success;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();

        internal void AddError(string message)
        {
            Errors.Add(new ErrorDto(message));
        }

        internal void AddError(int? statusCode, string message)
        {
            Errors.Add(new ErrorDto(message, statusCode));
        }

        internal void SetData(T data)
        {
            Data = data;
        }

        internal void SetSuccess(bool success)
        {
            Success = success;
        }
    }
}
