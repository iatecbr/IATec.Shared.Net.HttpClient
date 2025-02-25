using System;
using System.Net.Http;
using IATec.Shared.HttpClient.Configurations;
using IATec.Shared.HttpClient.Extensions;
using IATec.Shared.HttpClient.Service;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace IATec.Shared.HttpClient.DependencyInjection
{
    public static class HttpClientDependencyInjection
    {
        public static IServiceCollection AddHttpClientService(
            this IServiceCollection services,
            Action<HttpClientPolicyConfiguration> configurePolicy)
        {
            var config = new HttpClientPolicyConfiguration();

            configurePolicy?.Invoke(config);

            services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(serviceProvider =>
            {
                return GetCombinedPolicy(config);
            });

            services.AddHttpClient<IServiceClient, ServiceClient>()
                .AddPolicyHandler((serviceProvider, request) =>
                {
                    return serviceProvider.GetRequiredService<IAsyncPolicy<HttpResponseMessage>>();
                });

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(HttpClientPolicyConfiguration config)
        {
            IAsyncPolicy<HttpResponseMessage> policy = Policy.NoOpAsync<HttpResponseMessage>();

            if (config.UseRetry && config.UseCircuitBreaker)
            {
                var retryPolicy = PollyExtensions.RetryPolicy(config.RetryCount, config.RetryDelay);

                var circuitBreakerPolicy = CircuitBreakerExtensions
                    .CircuitBreakerPolicy(config.CircuitBreakerFailuresAllowedBeforeBreaking, config.CircuitBreakerDuration);

                policy = Policy.WrapAsync(policy, circuitBreakerPolicy);
            }

            if (config.UseRetry)
                policy = PollyExtensions.RetryPolicy(config.RetryCount, config.RetryDelay);

            if (config.UseCircuitBreaker)
                policy = CircuitBreakerExtensions
                    .CircuitBreakerPolicy(config.CircuitBreakerFailuresAllowedBeforeBreaking, config.CircuitBreakerDuration);

            return policy;
        }
    }
}
