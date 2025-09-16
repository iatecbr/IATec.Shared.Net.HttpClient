using IATec.Shared.HttpClient.Configurations;
using IATec.Shared.HttpClient.Extensions;
using IATec.Shared.HttpClient.Helpers;
using IATec.Shared.HttpClient.Resources;
using IATec.Shared.HttpClient.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Polly;
using System;
using System.Linq;
using System.Net.Http;

namespace IATec.Shared.HttpClient.DependencyInjection
{
    public static class HttpClientDependencyInjection
    {
        /// <summary>
        /// Adds the HttpClient service with configurable resiliency policies (Retry, Circuit Breaker, Timeout).
        /// <para>
        /// <b>Parameters:</b>
        /// <list type="bullet">
        ///   <item>
        ///     <description><paramref name="configurePolicy"/>: Delegate to configure resiliency policies via <see cref="HttpClientPolicyConfiguration"/>.</description>
        ///   </item>
        ///   <item>
        ///     <description><paramref name="configureClient"/>: Optional delegate to configure the <see cref="System.Net.Http.HttpClient"/>.</description>
        ///   </item>
        ///   <item>
        ///     <description><paramref name="clientName"/>: Http client name. Default: <see cref="Constants.DefaultHttpClientName"/>.</description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// <b>Configuration (<see cref="HttpClientPolicyConfiguration"/>):</b>
        /// <list type="bullet">
        ///   <item>
        ///     <description><c>UseRetry</c>: Enables retry policy.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>RetryCount</c>: Number of retry attempts.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>RetryDelay</c>: Delay between retry attempts.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>UseCircuitBreaker</c>: Enables circuit breaker policy.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>CircuitBreakerFailuresAllowedBeforeBreaking</c>: Failures allowed before opening the circuit.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>CircuitBreakerDuration</c>: Duration the circuit remains open.</description>
        ///   </item>
        ///   <item>
        ///     <description><c>RequestTimeout</c>: Request timeout (optional).</description>
        ///   </item>
        /// </list>
        /// </para>
        /// </summary>

        public static IServiceCollection AddHttpClientService(
            this IServiceCollection services,
            Action<HttpClientPolicyConfiguration> configurePolicy,
            Action<System.Net.Http.HttpClient>? configureClient = null,
            string clientName = Constants.DefaultHttpClientName)
        {
            if (services.All(sd => sd.ServiceType != typeof(IStringLocalizerFactory)))
                services.AddLocalization(options => options.ResourcesPath = "");

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var localizer = serviceProvider.GetRequiredService<IStringLocalizer<Messages>>();

                var config = new HttpClientPolicyConfiguration();
                configurePolicy.Invoke(config);
                var combinedPolicy = GetCombinedPolicy(config, localizer);

                services.AddHttpClient(clientName, configureClient ?? (_ => { }))
                    .AddPolicyHandler(combinedPolicy);
            }

            services.TryAddSingleton<IServiceClientFactory, ServiceClientFactory>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(
            HttpClientPolicyConfiguration config, IStringLocalizer<Messages> localizer)
        {
            IAsyncPolicy<HttpResponseMessage> policy = Policy.NoOpAsync<HttpResponseMessage>();

            if (config is { UseRetry: true, UseCircuitBreaker: true })
            {
                var retryPolicy = RetryExtensions.RetryPolicy(config.RetryCount, config.RetryDelay, localizer);

                var circuitBreakerPolicy = CircuitBreakerExtensions
                    .CircuitBreakerPolicy(config.CircuitBreakerFailuresAllowedBeforeBreaking,
                        config.CircuitBreakerDuration, localizer);

                var circuitBreakerTooManyRequestPolicy = CircuitBreakerExtensions
                    .CircuitBreakerTooManyRequestPolicy(config.CircuitBreakerDuration, localizer);

                policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy,
                    circuitBreakerTooManyRequestPolicy);

                return policy;
            }

            if (config.UseRetry)
                policy = RetryExtensions.RetryPolicy(config.RetryCount, config.RetryDelay, localizer);

            if (!config.UseCircuitBreaker) return policy;
            {
                var circuitBreakerPolicy = CircuitBreakerExtensions
                    .CircuitBreakerPolicy(config.CircuitBreakerFailuresAllowedBeforeBreaking,
                        config.CircuitBreakerDuration, localizer);

                var circuitBreakerTooManyRequestPolicy = CircuitBreakerExtensions
                    .CircuitBreakerTooManyRequestPolicy(config.CircuitBreakerDuration, localizer);

                policy = Policy.WrapAsync(circuitBreakerPolicy,
                    circuitBreakerTooManyRequestPolicy);
            }

            if (config.RequestTimeout.HasValue && config.RequestTimeout.Value > TimeSpan.Zero)
            {
                var timeoutPolicy = TimeoutExtensions.TimeoutPolicy(config.RequestTimeout.Value, localizer);
                policy = Policy.WrapAsync(timeoutPolicy, policy);
            }

            return policy;
        }
    }
}