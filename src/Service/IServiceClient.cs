using IATec.Shared.HttpClient.Dto;
using System.Net.Http;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service
{
    /// <summary>
    /// Defines the contract for an HTTP service client that performs basic CRUD operations.
    /// </summary>
    public interface IServiceClient
    {
        /// <summary>
        /// Sends an HTTP GET request to the specified URL and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        Task<ResponseDto<T>> GetAsync<T>(string url) where T : class;

        /// <summary>
        /// Sends an HTTP GET request to the specified URL and returns the raw string response.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the raw string response.</returns>
        Task<ResponseDto<string>> GetStringAsync(string url);

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the given content and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        Task<ResponseDto<T>> PostAsync<T>(string url, HttpContent content) where T : class;

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the given content.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<BaseResponseDto> PostAsync(string url, HttpContent content);

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with the given content and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        Task<ResponseDto<T>> PutAsync<T>(string url, HttpContent content) where T : class;

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with the given content.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<BaseResponseDto> PutAsync(string url, HttpContent content);

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URL and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        Task<ResponseDto<T>> DeleteAsync<T>(string url) where T : class;

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<BaseResponseDto> DeleteAsync(string url);
    }
}
