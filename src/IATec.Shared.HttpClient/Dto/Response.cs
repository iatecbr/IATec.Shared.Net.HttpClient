using System.Collections.Generic;

namespace IATec.Shared.HttpClient.Dto
{
    public class ResponseDto<T>
    {
        public ResponseDto(bool success)
        {
            Success = success;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
        public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();

        public void AddError(string message)
        {
            Errors.Add(new ErrorDto(message));
        }

        public void AddError(int? statusCode, string message)
        {
            Errors.Add(new ErrorDto(message, statusCode));
        }

        public void SetSuccess(bool success)
        {
            Success = success;
        }
    }
}
