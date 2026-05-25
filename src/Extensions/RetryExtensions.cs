using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring retry policies.
    /// </summary>
    public static class RetryExtensions
    {
        /// <summary>
        /// Creates a retry policy that handles transient HTTP failures.
        /// </summary>
        /// <param name="retryCount">The maximum number of retry attempts.</param>
        /// <param name="retryDelay">The delay between retry attempts.</param>
        /// <param name="localizer">The string localizer for messages.</param>
        /// <returns>An asynchronous retry policy for <see cref="HttpResponseMessage"/>.</returns>
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
