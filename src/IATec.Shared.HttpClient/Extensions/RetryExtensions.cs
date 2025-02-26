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
            return Policy
                .HandleResult<HttpResponseMessage>(r =>
                    r.StatusCode == HttpStatusCode.InternalServerError ||
                    r.StatusCode == HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => retryDelay,
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine(localizer
                            .GetString(nameof(Messages.RetryAttemptMessage), retryAttempt, timespan.TotalSeconds));
                    });
        }
    }
}
