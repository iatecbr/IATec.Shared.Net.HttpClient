using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Service
{
    /// <summary>
    /// Factory implementation for creating <see cref="IServiceClient"/> instances.
    /// </summary>
    internal class ServiceClientFactory : IServiceClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IStringLocalizer<Messages> _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClientFactory"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="localizer">The string localizer for messages.</param>
        public ServiceClientFactory(IHttpClientFactory httpClientFactory,
                                    IStringLocalizer<Messages> localizer)
        {
            _httpClientFactory = httpClientFactory;
            _localizer = localizer;
        }

        /// <summary>
        /// Creates a new <see cref="IServiceClient"/> instance using the specified client name.
        /// </summary>
        /// <param name="clientName">The logical name of the HTTP client.</param>
        /// <returns>An implementation of <see cref="IServiceClient"/>.</returns>
        public IServiceClient Create(string clientName)
            => new ServiceClient(_httpClientFactory.CreateClient(clientName), _localizer);
    }
}
