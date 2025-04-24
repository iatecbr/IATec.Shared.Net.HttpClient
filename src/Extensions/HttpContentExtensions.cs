using System.Net.Http;
using System.Text.Json;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class HttpContentExtensions
    {
        public static StringContent GenerateStringContent(this object content, string mediaType)
        {
            var json = JsonSerializer.Serialize(content);
            return new StringContent(json, System.Text.Encoding.UTF8, mediaType);
        }
    }
}