using IATec.Shared.HttpClient.Dto;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service
{
    public interface IServiceClient
    {
        Task<ResponseDto<T>> GetAsync<T>(string url) where T : class;
    }
}
