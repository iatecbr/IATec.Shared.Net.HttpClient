namespace IATec.Shared.HttpClient.Dto
{
    public class ErrorDto
    {
        public ErrorDto(string message)
        {
            Message = message;
        }

        public ErrorDto(string message, int? statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }

        public int? StatusCode { get; set; }
        public string Message { get; set; }
    }
}
