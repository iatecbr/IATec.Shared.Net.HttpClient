using IATec.Shared.HttpClient.Dto;
using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly.CircuitBreaker;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service
{
    public class ServiceClient : IServiceClient
    {
        private System.Net.Http.HttpClient _httpClient;
        private readonly IStringLocalizer<Messages> _localizer;

        public ServiceClient(System.Net.Http.HttpClient httpClient, IStringLocalizer<Messages> localizer)
        {
            _httpClient = httpClient;
            _localizer = localizer;
        }

        public async Task<ResponseDto<T>> GetAsync<T>(string url) where T : class
        {
            var responseDto = new ResponseDto<T>(false);

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (BrokenCircuitException)
            {
                responseDto.AddError(_localizer.GetString(ErrorMessages.CircuitBreakerIsOpen));
                return responseDto;
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(ErrorMessages.RequestError)}: {ex.Message}");
                return responseDto;
            }

            try
            {
                HandleResponse(response, responseDto);

                if (responseDto.Success)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseData);
                    responseDto.SetData(data);
                }
            }
            catch (Exception ex)
            {
                responseDto.AddError($"{_localizer.GetString(ErrorMessages.DeserializationError)}: {ex.Message}");
                responseDto.SetSuccess(false);
            }

            return responseDto;
        }

        private void HandleResponse<T>(HttpResponseMessage response, ResponseDto<T> responseDto)
        {
            if (response.IsSuccessStatusCode)
            {
                responseDto.SetSuccess(true);
                return;
            }

            responseDto.SetSuccess(false);
            responseDto.AddError((int)response.StatusCode, response.ReasonPhrase);
        }
    }
}
