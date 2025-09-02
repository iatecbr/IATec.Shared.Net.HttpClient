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

    public async Task<ResponseDto<T>> GetAsync<T>(string url) where T : class
    {
        return await GetAsync<T,ErrorResponseDto>(url);
    }

    public async Task<ResponseDto<T>> GetAsync<T,TError>(string url) where T : class
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
            await HandleResponse<T,TError>(response, responseDto);

            if (responseDto.Success)
                responseDto.SetData(await Deserialize<T>(response));
        }
        catch (Exception ex)
        {
            responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
            responseDto.SetSuccess(false);
        }

        return responseDto;
    }

    public async Task<ResponseDto<string>> GetStringAsync(string url)
    {
        return await GetStringAsync<ErrorResponseDto>(url);
    }

    public async Task<ResponseDto<string>> GetStringAsync<TError>(string url)
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

    public async Task<BaseResponseDto> PostAsync(string url, HttpContent content)
    {
        return await PostAsync<BaseResponseDto, ErrorResponseDto>(url, content);
    }

    public record nullDto;

    public async Task<ResponseDto<T>> PostAsync<T>(string url, HttpContent content) where T : class
    {
        if (typeof(T) == typeof(nullDto));
            return await PostAsync<nullDto,T>(url, content);
    }

    public async Task<ResponseDto<T?>> PostAsync<T, TError>(string url, HttpContent content) where T : class where TError : class
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
            await HandleResponse<T,TError>(response, responseDto);

            if (responseDto.Success)
                responseDto.SetData(await Deserialize<T>(response));
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

    public async Task<ResponseDto<T>> PutAsync<T>(string url, HttpContent content) where T : class
    {        
        return await PutAsync<T, ErrorResponseDto>(url, content);
    }

    public async Task<ResponseDto<T>> PutAsync<T, TError>(string url, HttpContent content) where T : class
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
            await HandleResponse<T,TError>(response, responseDto);

            if (responseDto.Success)
                responseDto.SetData(await Deserialize<T>(response));
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

    public async Task<ResponseDto<T>> DeleteAsync<T>(string url) where T : class
    {                
        var http = _httpClient.PostAsync(url, null);
        return await DeleteAsync<T, ErrorResponseDto>(url);        
    }    

    public async Task<ResponseDto<T>> DeleteAsync<T, TError>(string url) where T : class
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
            await HandleResponse<T,TError>(response, responseDto);            
        }
        catch (Exception ex)
        {
            responseDto.AddError($"{_localizer.GetString(nameof(Messages.DeserializationError))}: {ex.Message}");
            responseDto.SetSuccess(false);
        }

        return responseDto;
    }

    private async Task HandleResponse<T, TError>(HttpResponseMessage response, ResponseDto<T> responseDto)
    {
        if (response.IsSuccessStatusCode)
        {            
            responseDto.SetSuccess(true);

            responseDto.SetData(await Deserialize<T>(response));
            return;
        }

        var localizedResponseError = GetStatusCodeMessages(response.StatusCode, responseDto);

        if (localizedResponseError != null)
        {
            responseDto.AddError((int)response.StatusCode, localizedResponseError);
            return;
        }

        var errorResponse = await Deserialize<TError>(response);

        if (typeof(TError) == typeof(ErrorResponseDto))
        {
            var errorResponseDto = errorResponse as ErrorResponseDto;
            if (errorResponseDto?.Messages.Count > 0)
                responseDto.AddRangeError((int)response.StatusCode, errorResponseDto.Messages);

            else                
                responseDto.AddError((int)HttpStatusCode.BadRequest, _localizer.GetString(nameof(Messages.BadRequest)));
        }
        else
            responseDto.AddError((int)response.StatusCode, $"{JsonSerializer.Serialize(errorResponse)}");
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

        return JsonSerializer.Deserialize<T>(responseData, options)!;
    }

    //private async Task<object?> DeserializeError<TError>(HttpResponseMessage response)
    //{
    //    var options = new JsonSerializerOptions
    //    {
    //        PropertyNameCaseInsensitive = true
    //    };

    //    var responseData = await response.Content.ReadAsStringAsync();
        
    //    return JsonSerializer.Deserialize<TError>(responseData, options);            
    //}
}