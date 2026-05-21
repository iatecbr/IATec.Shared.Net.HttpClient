using System.Net.Http;
using System.Text.Json;

namespace IATec.Shared.HttpClient.Extensions
{
    /// <summary>
    /// Provides extension methods for creating <see cref="StringContent"/> from objects.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Serializes the specified object to JSON and creates a <see cref="StringContent"/> instance.
        /// </summary>
        /// <param name="content">The object to serialize.</param>
        /// <param name="mediaType">The media type header value.</param>
        /// <returns>A <see cref="StringContent"/> containing the serialized JSON.</returns>
        public static StringContent GenerateStringContent(this object content, string mediaType)
        {
            var json = JsonSerializer.Serialize(content, SerializerExtensions.SerializerOptions);
            return new StringContent(json, System.Text.Encoding.UTF8, mediaType);
        }
    }
}
