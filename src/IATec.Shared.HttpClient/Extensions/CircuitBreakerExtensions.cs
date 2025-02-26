using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class CircuitBreakerExtensions
    {
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy(
            int allowedBeforeBreaking, TimeSpan circuitBreakerDuration, IStringLocalizer<Messages> localizer)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(allowedBeforeBreaking, circuitBreakerDuration,
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine(localizer.GetString("Circuit breaker opened for {0} seconds", breakDelay.TotalSeconds));
                    },
                    onReset: () =>
                    {
                        Console.WriteLine(localizer.GetString("Circuit breaker reset"));
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine(localizer.GetString("Circuit breaker half-open"));
                    });
        }
    }
}
