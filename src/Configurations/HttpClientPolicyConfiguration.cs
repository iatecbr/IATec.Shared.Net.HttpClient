using System;

namespace IATec.Shared.HttpClient.Configurations
{
    /// <summary>
    /// Represents the configuration settings for HttpClient policies, including Retry and Circuit Breaker.
    /// TooManyRequest status code is handled by default on Circuit Breaker. 
    /// </summary>
    public class HttpClientPolicyConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Circuit Breaker policy should be used.
        /// Default value: false.
        /// </summary>
        public bool UseCircuitBreaker { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Retry policy (with Polly) should be used.
        /// Default value: false.
        /// </summary>
        public bool UseRetry { get; set; } = false;

        /// <summary>
        /// Gets or sets the number of retry attempts to be made in case a request fails.
        /// Default value: 3.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the delay between each retry attempt.
        /// Default value: 2 seconds.
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Gets or sets the number of consecutive failures allowed before triggering the Circuit Breaker.
        /// Default value: 4.
        /// </summary>
        public int CircuitBreakerFailuresAllowedBeforeBreaking { get; set; } = 4;

        /// <summary>
        /// Gets or sets the duration for which the Circuit Breaker will remain open after being triggered.
        /// Default value: 30 seconds.
        /// </summary>
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
    }
}
