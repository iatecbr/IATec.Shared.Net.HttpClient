using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net;
using System.Net.Http;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class CircuitBreakerExtensions
    {
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy(int allowedBeforeBreaking, TimeSpan circuitBreakerDuration)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .CircuitBreakerAsync(
                allowedBeforeBreaking, circuitBreakerDuration,
                onBreak: (outcome, breakDelay) => { Console.WriteLine($"Circuit breaker opened for {breakDelay.TotalSeconds} seconds"); },
                onReset: () => { Console.WriteLine("Circuit breaker reset"); },
                onHalfOpen: () => { Console.WriteLine("Circuit breaker half-open"); });
        }
    }
}
