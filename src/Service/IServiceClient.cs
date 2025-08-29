using IATec.Shared.HttpClient.Dto;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service
{
    public interface IServiceClient
    {

        Task<ResponseDto<T>> GetAsync<T>(string url) where T : class;
        Task<ResponseDto<T>> GetAsync<T>(string url, Type? error) where T : class;

        Task<ResponseDto<string>> GetStringAsync(string url);
        Task<ResponseDto<string>> GetStringAsync(string url, Type? error);

        Task<ResponseDto<T>> PostAsync<T>(string url, HttpContent content) where T : class;
        Task<ResponseDto<T>> PostAsync<T>(string url, HttpContent content, Type? error) where T : class;

        Task<BaseResponseDto> PostAsync(string url, HttpContent content);
        Task<BaseResponseDto> PostAsync(string url, HttpContent content, Type? error);

        Task<ResponseDto<T>> PutAsync<T>(string url, HttpContent content) where T : class;
        Task<ResponseDto<T>> PutAsync<T>(string url, HttpContent content, Type? error) where T : class;

        Task<BaseResponseDto> PutAsync(string url, HttpContent content);
        Task<BaseResponseDto> PutAsync(string url, HttpContent content, Type? error);

        Task<ResponseDto<T>> DeleteAsync<T>(string url) where T : class;
        Task<ResponseDto<T>> DeleteAsync<T>(string url, Type? error) where T : class;

        Task<BaseResponseDto> DeleteAsync(string url);
        Task<BaseResponseDto> DeleteAsync(string url, Type? error);
    }    
}