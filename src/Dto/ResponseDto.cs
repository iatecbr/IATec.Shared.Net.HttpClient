namespace IATec.Shared.HttpClient.Dto
{
    /// <summary>
    /// Represents an HTTP response containing typed data.
    /// </summary>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public class ResponseDto<T> : BaseResponseDto
    {
        /// <summary>
        /// Gets the deserialized data returned by the HTTP request.
        /// </summary>
        public T Data { get; private set; }

        internal void SetData(T data)
        {
            Data = data;
        }
    }
}
