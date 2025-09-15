using IATec.Shared.HttpClient.Helpers;

namespace IATec.Shared.HttpClient.Service
{
    public interface IServiceClientFactory
    {
        IServiceClient Create(string clientName = Constants.DefaultHttpClientName);
    }
}