using System;

namespace IATec.Shared.HttpClient.Configurations
{
    public class HttpClientPolicyConfiguration
    {
        public bool UseCircuitBreaker { get; set; } = false;

        public bool UseRetry { get; set; } = false;

        public int RetryCount { get; set; } = 3;

        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);

        public int CircuitBreakerFailuresAllowedBeforeBreaking { get; set; } = 1;

        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(2);
    }
}
