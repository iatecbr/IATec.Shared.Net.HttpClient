using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring timeout policies.
    /// </summary>
    public static class TimeoutExtensions
    {
        /// <summary>
        /// Creates a timeout policy for HTTP requests.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <param name="localizer">The string localizer for messages.</param>
        /// <returns>An asynchronous timeout policy for <see cref="HttpResponseMessage"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy(
            TimeSpan timeout,
            IStringLocalizer<Messages> localizer
            )
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                timeout,
                TimeoutStrategy.Optimistic,
                onTimeoutAsync: async (context, span, _, ex) =>
                {
                    Console.WriteLine(localizer
                            .GetString(nameof(Messages.TimeoutMessage)));

                    await Task.CompletedTask;
                });
        }
    }
}
