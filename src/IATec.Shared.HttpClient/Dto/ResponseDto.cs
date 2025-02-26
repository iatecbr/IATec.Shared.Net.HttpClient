using System.Collections.Generic;

namespace IATec.Shared.HttpClient.Dto
{
    public class ResponseDto<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; }
        private readonly List<ErrorDto> _errors = new List<ErrorDto>();
        public IReadOnlyList<ErrorDto> Errors => _errors.AsReadOnly();

        internal void AddError(string message)
        {
            _errors.Add(new ErrorDto(message));
        }

        internal void AddError(int? statusCode, string message)
        {
            _errors.Add(new ErrorDto(message, statusCode));
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
