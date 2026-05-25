using System.Collections.Generic;

namespace IATec.Shared.HttpClient.Dto
{
    /// <summary>
    /// Represents the error response payload deserialized from an HTTP error response.
    /// </summary>
    public class ErrorResponseDto
    {
        /// <summary>
        /// Gets the list of error messages returned by the server.
        /// </summary>
        public List<string> Messages { get; set; } = new List<string>();
    }
}
