using IATec.Shared.HttpClient.Dto;
using System.Net.Http;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service
{
    public interface IServiceClient
    {
        Task<ResponseDto<T>> GetAsync<T>(string url) where T : class;

        Task<ResponseDto<string>> GetStringAsync(string url);

        Task<ResponseDto<T>> PostAsync<T>(string url, HttpContent content) where T : class;

        Task<BaseResponseDto> PostAsync(string url, HttpContent content);

        Task<ResponseDto<T>> PutAsync<T>(string url, HttpContent content) where T : class;

        Task<BaseResponseDto> PutAsync(string url, HttpContent content);

        Task<ResponseDto<T>> DeleteAsync<T>(string url) where T : class;

        Task<BaseResponseDto> DeleteAsync(string url);
    }
}