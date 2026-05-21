using IATec.Shared.HttpClient.Dto;
using IATec.Shared.HttpClient.Extensions;
using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service
{
    /// <summary>
    /// Provides HTTP request methods with built-in resiliency policies and error handling.
    /// </summary>
    internal class ServiceClient : IServiceClient
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        private readonly IStringLocalizer<Messages> _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClient"/> class.
        /// </summary>
        /// <param name="httpClient">The underlying HTTP client.</param>
        /// <param name="localizer">The string localizer for messages.</param>
        public ServiceClient(System.Net.Http.HttpClient httpClient, IStringLocalizer<Messages> localizer)
        {
            _httpClient = httpClient;
            _localizer = localizer;
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URL and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        public async Task<ResponseDto<T>> GetAsync<T>(string url) where T : class
        {
            var responseDto = new ResponseDto<T>();

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                await HandleResponseAsync(response, responseDto);

                if (responseDto.Success)
                    responseDto.SetData(await DeserializeAsync<T>(response));
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URL and returns the raw string response.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the raw string response.</returns>
        public async Task<ResponseDto<string>> GetStringAsync(string url)
        {
            var responseDto = new ResponseDto<string>();

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    responseDto.SetData(content);
                    responseDto.SetSuccess(true);
                }

                var localizedResponseError = GetStatusCodeMessages(response.StatusCode, responseDto);

                if (localizedResponseError != null)
                    responseDto.AddError((int)response.StatusCode, localizedResponseError);
                else
                    responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the given content and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        public async Task<ResponseDto<T>> PostAsync<T>(string url, HttpContent content) where T : class
        {
            var responseDto = new ResponseDto<T>();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(url, content);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                await HandleResponseAsync(response, responseDto);

                if (responseDto.Success)
                    responseDto.SetData(await DeserializeAsync<T>(response));
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        /// <summary>
        /// Sends an HTTP POST request to the specified URL with the given content.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<BaseResponseDto> PostAsync(string url, HttpContent content)
        {
            var responseDto = new BaseResponseDto();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(url, content);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                await HandleResponseAsync(response, responseDto);
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with the given content and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        public async Task<ResponseDto<T>> PutAsync<T>(string url, HttpContent content) where T : class
        {
            var responseDto = new ResponseDto<T>();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PutAsync(url, content);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                await HandleResponseAsync(response, responseDto);

                if (responseDto.Success)
                    responseDto.SetData(await DeserializeAsync<T>(response));
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        /// <summary>
        /// Sends an HTTP PUT request to the specified URL with the given content.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <param name="content">The HTTP content to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<BaseResponseDto> PutAsync(string url, HttpContent content)
        {
            var responseDto = new BaseResponseDto();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PutAsync(url, content);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                await HandleResponseAsync(response, responseDto);
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URL and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response data.</returns>
        public async Task<ResponseDto<T>> DeleteAsync<T>(string url) where T : class
        {
            var responseDto = new ResponseDto<T>();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.DeleteAsync(url);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                await HandleResponseAsync(response, responseDto);

                if (responseDto.Success)
                    responseDto.SetData(await DeserializeAsync<T>(response));
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<BaseResponseDto> DeleteAsync(string url)
        {
            var responseDto = new BaseResponseDto();
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.DeleteAsync(url);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(nameof(Messages.CircuitBreakerOpenTryAgainLater)));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.RequestError))}: {ex.Message}");
                return responseDto;
            }

            try
            {
                await HandleResponseAsync(response, responseDto);
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        private async Task HandleResponseAsync(
            HttpResponseMessage response,
            BaseResponseDto responseDto
        )
        {
            if (response.IsSuccessStatusCode)
            {
                responseDto.SetSuccess(true);
                return;
            }

            var localizedResponseError = GetStatusCodeMessages(response.StatusCode, responseDto);

            if (localizedResponseError != null)
            {
                responseDto.AddError((int)response.StatusCode, localizedResponseError);
                return;
            }

            var errorResponseDto = await DeserializeAsync<ErrorResponseDto>(response);

            if (errorResponseDto.Messages.Count == 0)
                responseDto.AddError((int)HttpStatusCode.BadRequest, _localizer.GetString(nameof(Messages.BadRequest)));
            else
                responseDto.AddRangeError((int)response.StatusCode, errorResponseDto.Messages);
        }

        private string? GetStatusCodeMessages(HttpStatusCode responseStatusCode, BaseResponseDto responseDto)
        {
            responseDto.SetSuccess(false);
            var statusCodeMessages = new Dictionary<HttpStatusCode, string>
            {
                [HttpStatusCode.Unauthorized] = _localizer.GetString(nameof(Messages.Unauthorized)),
                [HttpStatusCode.NotFound] = _localizer.GetString(nameof(Messages.NotFound)),
                [HttpStatusCode.Forbidden] = _localizer.GetString(nameof(Messages.Forbidden)),
                [HttpStatusCode.InternalServerError] = _localizer.GetString(nameof(Messages.InternalServerError))
            };

            return statusCodeMessages.GetValueOrDefault(responseStatusCode);
        }

        private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseData, SerializerExtensions.SerializerOptions)!;
        }
    }
}
