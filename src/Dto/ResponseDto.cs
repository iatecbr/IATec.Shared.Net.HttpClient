namespace IATec.Shared.HttpClient.Dto
{
    /// <summary>
    /// Represents an HTTP response containing typed data.
    /// </summary>
    /// <remarks>
    /// The <see cref="Data"/> property is only populated when <see cref="BaseResponseDto.Success"/> is <c>true</c>.
    /// When <see cref="BaseResponseDto.Success"/> is <c>false</c>, <see cref="Data"/> will be <c>null</c>.
    /// Consumers should always check <see cref="BaseResponseDto.Success"/> before accessing <see cref="Data"/>.
    /// </remarks>
    /// <typeparam name="T">The type of the response data.</typeparam>
    public class ResponseDto<T> : BaseResponseDto where T : class
    {
        /// <summary>
        /// Gets the deserialized data returned by the HTTP request.
        /// Only populated when <see cref="BaseResponseDto.Success"/> is <c>true</c>; otherwise <c>null</c>.
        /// </summary>
        public T? Data { get; private set; }

        /// <summary>
        /// Assigns the deserialized response data.
        /// </summary>
        /// <param name="data">The non-null deserialized data.</param>
        internal void SetData(T data)
        {
            Data = data;
        }
    }
}
