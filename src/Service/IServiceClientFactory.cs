using IATec.Shared.HttpClient.Helpers;

namespace IATec.Shared.HttpClient.Service
{
    /// <summary>
    /// Defines a factory for creating instances of <see cref="IServiceClient"/>.
    /// </summary>
    public interface IServiceClientFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IServiceClient"/> configured for the given client name.
        /// </summary>
        /// <param name="clientName">The logical name of the HTTP client. Defaults to <see cref="Constants.DefaultHttpClientName"/>.</param>
        /// <returns>An instance of <see cref="IServiceClient"/>.</returns>
        IServiceClient Create(string clientName = Constants.DefaultHttpClientName);
    }
}
