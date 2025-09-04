using IATec.Shared.HttpClient.Dto;
using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Service;

public class ServiceClient : IServiceClient
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly IStringLocalizer<Messages> _localizer;

    public ServiceClient(System.Net.Http.HttpClient httpClient, IStringLocalizer<Messages> localizer)
    {
        _httpClient = httpClient;
        _localizer = localizer;
    }

    public async Task<ResponseDto<T, ErrorResponseDto>> GetAsync<T>(string url) where T : class
    {
        return await GetAsync<T, ErrorResponseDto>(url);
    }

    public async Task<ResponseDto<T, TError>> GetAsync<T, TError>(string url) where T : class
    {
        var responseDto = new ResponseDto<T, TError>();

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
            await HandleResponse<T, TError>(response, responseDto);
        }
        catch (Exception ex)
        {
            responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
            responseDto.SetSuccess(false);
        }

        return responseDto;
    }

    public async Task<ResponseDto<string, ErrorResponseDto>> GetStringAsync(string url)
    {
        return await GetStringAsync<ErrorResponseDto>(url);
    }

    public async Task<ResponseDto<string, TError>> GetStringAsync<TError>(string url)
    {
        var responseDto = new ResponseDto<string, TError>();

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

    public async Task<BaseResponseDto> PostAsync(string url, HttpContent content)
    {
        return await PostAsync<BaseResponseDto, ErrorResponseDto>(url, content);
    }


    public async Task<ResponseDto<T, ErrorResponseDto>> PostAsync<T>(string url, HttpContent content) where T : class
    {
        return await PostAsync<T, ErrorResponseDto>(url, content);
    }

    public async Task<ResponseDto<T, TError>> PostAsync<T, TError>(string url, HttpContent content) where T : class where TError : class
    {
        var responseDto = new ResponseDto<T, TError>();

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
            await HandleResponse(response, responseDto);
        }

        catch (Exception ex)
        {
            responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
            responseDto.SetSuccess(false);
        }

        return responseDto;
    }

    public async Task<BaseResponseDto> PutAsync(string url, HttpContent content)
    {
        return await PutAsync<BaseResponseDto, ErrorResponseDto>(url, content);
    }

    public async Task<ResponseDto<T, ErrorResponseDto>> PutAsync<T>(string url, HttpContent content) where T : class
    {
        return await PutAsync<T, ErrorResponseDto>(url, content);
    }

    public async Task<ResponseDto<T, TError>> PutAsync<T, TError>(string url, HttpContent content) where T : class where TError : class
    {
        var responseDto = new ResponseDto<T, TError>();

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
            await HandleResponse(response, responseDto);
        }
        catch (Exception ex)
        {
            responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
            responseDto.SetSuccess(false);
        }

        return responseDto;
    }

    public async Task<BaseResponseDto> DeleteAsync(string url)
    {
        return await DeleteAsync<BaseResponseDto, ErrorResponseDto>(url);
    }

    public async Task<ResponseDto<T, ErrorResponseDto>> DeleteAsync<T>(string url) where T : class
    {
        return await DeleteAsync<T, ErrorResponseDto>(url);
    }

    public async Task<ResponseDto<T, TError>> DeleteAsync<T, TError>(string url) where T : class where TError : class
    {
        var responseDto = new ResponseDto<T, TError>();

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
            await HandleResponse(response, responseDto);
        }
        catch (Exception ex)
        {
            responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
            responseDto.SetSuccess(false);
        }

        return responseDto;
    }

    private async Task HandleResponse<T, TError>(HttpResponseMessage response, ResponseDto<T, TError> responseDto)
    {
        if (response.IsSuccessStatusCode)
        {
            responseDto.SetSuccess(true);

            var responseBaseDto = await DeserializeSuccess(response);

            if (responseBaseDto != null)
            {
                responseDto.SetBaseData(responseBaseDto);
                return;
            }
            else
            {
                try
                {
                    responseDto.SetData(await Deserialize<T>(response));
                }
                catch (JsonException)
                {
                    return;
                }
            }

            return;
        }

        var localizedResponseError = GetStatusCodeMessages(response.StatusCode, responseDto);

        responseDto.SetSuccess(false);

        if (localizedResponseError != null)
        {
            responseDto.AddError((int)response.StatusCode, localizedResponseError);
            return;
        }

        TError? errorResponse = default;
        T? errorResponseFallback = default;
        bool errorHandled;

        try
        {
            errorResponse = await Deserialize<TError>(response);

            errorHandled = true;
        }
        catch (JsonException)
        {
            errorResponseFallback = await Deserialize<T>(response);
            errorHandled = false;
        }

        if (errorHandled)
        {
            if (typeof(TError) == typeof(ErrorResponseDto))
            {
                var errorResponseDto = errorResponse as ErrorResponseDto;
                if (errorResponseDto?.Messages?.Count > 0)
                    responseDto.AddRangeError((int)response.StatusCode, errorResponseDto.Messages);
                else
                    responseDto.AddError((int)HttpStatusCode.BadRequest, _localizer.GetString(nameof(Messages.BadRequest)));
            }
            else
                responseDto.SetError(errorResponse!);

        }
        else
            responseDto.SetData(errorResponseFallback!);
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

    private static async Task<T?> Deserialize<T>(HttpResponseMessage response)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var responseData = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<T>(responseData, options);

        if (typeof(T) == typeof(ErrorResponseDto))
        {
            if (result is not ErrorResponseDto errorDto || errorDto.Messages == null || errorDto.Messages.Count == 0)
                throw new JsonException($"Não foi possível desserializar para {typeof(T).Name} com dados relevantes. Conteúdo: {responseData}");
        }

        if (typeof(T) == typeof(BaseResponseDto))
        {
            if (result is not BaseResponseDto baseResponseDto || baseResponseDto.Success == false)
                throw new JsonException($"Não foi possível desserializar para {typeof(T).Name} com dados relevantes. Conteúdo: {responseData}");
        }

        else if (result == null)
            throw new JsonException($"Não foi possível desserializar o conteúdo para o tipo {typeof(T).Name}. Conteúdo: {responseData}");

        return result;
    }

    public async Task<BaseResponseDto> DeserializeSuccess(HttpResponseMessage response)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var responseData = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<BaseResponseDto>(responseData, options);

        if (result is BaseResponseDto successDto && successDto.Success)
            return result;

        return null;
    }
}