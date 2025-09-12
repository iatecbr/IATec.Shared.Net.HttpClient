using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Service
{
    public class ServiceClientFactory : IServiceClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IStringLocalizer<Messages> _localizer;

        public ServiceClientFactory(IHttpClientFactory httpClientFactory,
                                    IStringLocalizer<Messages> localizer)
        {
            _httpClientFactory = httpClientFactory;
            _localizer = localizer;
        }

        public IServiceClient Create(string clientName)
            => new ServiceClient(_httpClientFactory.CreateClient(clientName), _localizer);
    }
}