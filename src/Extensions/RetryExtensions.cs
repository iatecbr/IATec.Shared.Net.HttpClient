using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class RetryExtensions
    {
        public static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy(
            int retryCount, TimeSpan retryDelay, IStringLocalizer<Messages> localizer)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(response => response.StatusCode >= HttpStatusCode.InternalServerError ||
                                      response.StatusCode == HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => retryDelay,
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine(localizer
                            .GetString(nameof(Messages.RetryAttemptMessage), retryAttempt, timespan.TotalSeconds));
                    });
        }
    }
}