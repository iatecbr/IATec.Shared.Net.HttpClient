using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class CircuitBreakerExtensions
    {
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy(
            int allowedBeforeBreaking, TimeSpan circuitBreakerDuration, IStringLocalizer<Messages> localizer)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(response => response.StatusCode >= HttpStatusCode.InternalServerError ||
                                      response.StatusCode == HttpStatusCode.RequestTimeout)
                .CircuitBreakerAsync(allowedBeforeBreaking, circuitBreakerDuration,
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerOpenedForSeconds), 
                            breakDelay.TotalSeconds));
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
        
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerTooManyRequestPolicy(
            TimeSpan circuitBreakerDuration, IStringLocalizer<Messages> localizer)
        {

            return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .CircuitBreakerAsync(1, circuitBreakerDuration,
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine(localizer.GetString(nameof(Messages.CircuitBreakerOpenedForSeconds),
                                              breakDelay.TotalSeconds));
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
    }
}
