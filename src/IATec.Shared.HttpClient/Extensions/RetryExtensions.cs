using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
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
                .HandleResult<HttpResponseMessage>(r => HandleRequestResult(r.StatusCode))
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => retryDelay,
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine(localizer
                            .GetString(nameof(Messages.RetryAttemptMessage), retryAttempt, timespan.TotalSeconds));
                    });
        }

        private static bool HandleRequestResult(HttpStatusCode statusCode)
        {
            HttpStatusCode[] transientStatusCodes = new[]
            {
                HttpStatusCode.InternalServerError,
                HttpStatusCode.BadGateway,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.GatewayTimeout,
                HttpStatusCode.RequestTimeout
            };

            return transientStatusCodes.Contains(statusCode);
        }
    }
}
