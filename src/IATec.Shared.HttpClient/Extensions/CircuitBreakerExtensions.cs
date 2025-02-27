using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class CircuitBreakerExtensions
    {
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy(
            int allowedBeforeBreaking, TimeSpan circuitBreakerDuration, IStringLocalizer<Messages> localizer)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => HandleRequestResult(r.StatusCode))
                .CircuitBreakerAsync(allowedBeforeBreaking, circuitBreakerDuration,
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerOpenedForSeconds), breakDelay.TotalSeconds));
                    },
                    onReset: () =>
                    {
                        Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerReset)));
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerHalfOpen)));
                    });
        }

        private static bool HandleRequestResult(HttpStatusCode statusCode)
        {
            HttpStatusCode[] breakerStatusCodes = new[]
            {
                HttpStatusCode.TooManyRequests
            };

            return breakerStatusCodes.Contains(statusCode);
        }
    }
}
