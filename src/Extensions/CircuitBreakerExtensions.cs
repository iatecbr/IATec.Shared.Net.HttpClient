using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring circuit breaker policies.
    /// </summary>
    public static class CircuitBreakerExtensions
    {
        /// <summary>
        /// Creates a circuit breaker policy for transient HTTP failures.
        /// </summary>
        /// <param name="allowedBeforeBreaking">The number of failures allowed before breaking the circuit.</param>
        /// <param name="circuitBreakerDuration">The duration the circuit remains open.</param>
        /// <param name="localizer">The string localizer for messages.</param>
        /// <returns>An asynchronous circuit breaker policy for <see cref="HttpResponseMessage"/>.</returns>
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy(
            int allowedBeforeBreaking, TimeSpan circuitBreakerDuration, IStringLocalizer<Messages> localizer)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(response => response.StatusCode >= HttpStatusCode.InternalServerError ||
                                      response.StatusCode == HttpStatusCode.RequestTimeout)
                .CircuitBreakerAsync(allowedBeforeBreaking, circuitBreakerDuration,
                    (outcome, breakDelay) =>
                    {
                        Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerOpenedForSeconds),
                            breakDelay.TotalSeconds));
                    },
                    () => { Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerReset))); },
                    () => { Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerHalfOpen))); });
        }

        /// <summary>
        /// Creates a circuit breaker policy specifically for HTTP 429 (Too Many Requests) responses.
        /// </summary>
        /// <param name="circuitBreakerDuration">The duration the circuit remains open.</param>
        /// <param name="localizer">The string localizer for messages.</param>
        /// <returns>An asynchronous circuit breaker policy for <see cref="HttpResponseMessage"/>.</returns>
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerTooManyRequestPolicy(
            TimeSpan circuitBreakerDuration, IStringLocalizer<Messages> localizer)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .CircuitBreakerAsync(1, circuitBreakerDuration,
                    (outcome, breakDelay) =>
                    {
                        Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerOpenedForSeconds),
                            breakDelay.TotalSeconds));
                    },
                    () => { Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerReset))); },
                    () => { Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerHalfOpen))); });
        }
    }
}
