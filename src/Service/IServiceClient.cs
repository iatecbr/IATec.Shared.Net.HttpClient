using IATec.Shared.HttpClient.Dto;
using System.Net.Http;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service
{
    public interface IServiceClient
    {
        Task<ResponseDto<T, ErrorResponseDto>> GetAsync<T>(string url) where T : class;
        Task<ResponseDto<T, TError>> GetAsync<T, TError>(string url) where T : class;

        Task<ResponseDto<string, ErrorResponseDto>> GetStringAsync(string url);
        Task<ResponseDto<string, TError>> GetStringAsync<TError>(string url);

        Task<BaseResponseDto> PostAsync(string url, HttpContent content);
        Task<ResponseDto<T, ErrorResponseDto>> PostAsync<T>(string url, HttpContent content) where T : class;
        Task<ResponseDto<T, TError>> PostAsync<T, TError>(string url, HttpContent content) where T : class where TError : class;

        Task<BaseResponseDto> PutAsync(string url, HttpContent content);
        Task<ResponseDto<T, ErrorResponseDto>> PutAsync<T>(string url, HttpContent content) where T : class;
        Task<ResponseDto<T, TError>> PutAsync<T, TError>(string url, HttpContent content) where T : class where TError : class;

        Task<BaseResponseDto> DeleteAsync(string url);
        Task<ResponseDto<T, ErrorResponseDto>> DeleteAsync<T>(string url) where T : class;
        Task<ResponseDto<T, TError>> DeleteAsync<T, TError>(string url) where T : class where TError : class;
    }
}