namespace IATec.Shared.HttpClient.Dto
{
    public class ErrorDto
    {
        internal ErrorDto(string message)
        {
            Message = message;
        }

        internal ErrorDto(string message, int? statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }

        public int? StatusCode { get; set; }
        public string Message { get; set; }
    }
}
