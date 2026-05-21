using System.Text.Json;
using System.Text.Json.Serialization;

namespace IATec.Shared.HttpClient.Extensions
{
    /// <summary>
    /// Provides pre-configured JSON serializer options for HTTP content serialization.
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Gets the default JSON serializer options used across the library.
        /// </summary>
        public static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

        private static JsonSerializerOptions CreateSerializerOptions()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            return options;
        }
    }
}
