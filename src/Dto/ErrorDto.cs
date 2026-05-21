namespace IATec.Shared.HttpClient.Dto
{
    /// <summary>
    /// Represents an individual error with an optional HTTP status code.
    /// </summary>
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

        /// <summary>
        /// Gets the HTTP status code associated with the error, if any.
        /// </summary>
        public int? StatusCode { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; private set; }
    }
}
