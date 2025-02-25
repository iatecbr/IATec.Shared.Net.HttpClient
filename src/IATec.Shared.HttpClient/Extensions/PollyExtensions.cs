using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class PollyExtensions
    {
        public static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy(int retryCount, TimeSpan retryDelay)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && r.StatusCode != HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => retryDelay,
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"Retry attempt {retryAttempt} after {timespan.TotalSeconds} sec");
                    });
        }
    }
}
