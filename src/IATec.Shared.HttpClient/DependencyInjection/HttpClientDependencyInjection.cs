using System;
using System.Linq;
using System.Net.Http;
using IATec.Shared.HttpClient.Configurations;
using IATec.Shared.HttpClient.Extensions;
using IATec.Shared.HttpClient.Resources;
using IATec.Shared.HttpClient.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Polly;

namespace IATec.Shared.HttpClient.DependencyInjection
{
    public static class HttpClientDependencyInjection
    {
        public static IServiceCollection AddHttpClientService(
            this IServiceCollection services,
            Action<HttpClientPolicyConfiguration> configurePolicy)
        {
            if (!services.Any(sd => sd.ServiceType == typeof(IStringLocalizerFactory)))
                services.AddLocalization(options => options.ResourcesPath = "");

            var config = new HttpClientPolicyConfiguration();

            configurePolicy.Invoke(config);

            services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(serviceProvider =>
            {
                var localizer = serviceProvider.GetRequiredService<IStringLocalizer<Messages>>();
                return GetCombinedPolicy(config, localizer);
            });

            services.AddHttpClient<IServiceClient, ServiceClient>()
                .AddPolicyHandler((serviceProvider, request) =>
                {
                    return serviceProvider.GetRequiredService<IAsyncPolicy<HttpResponseMessage>>();
                });

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(
            HttpClientPolicyConfiguration config, IStringLocalizer<Messages> localizer)
        {
            IAsyncPolicy<HttpResponseMessage> policy = Policy.NoOpAsync<HttpResponseMessage>();

            if (config.UseRetry && config.UseCircuitBreaker)
            {
                var retryPolicy = PollyExtensions.RetryPolicy(config.RetryCount, config.RetryDelay, localizer);

                var circuitBreakerPolicy = CircuitBreakerExtensions
                    .CircuitBreakerPolicy(config.CircuitBreakerFailuresAllowedBeforeBreaking, config.CircuitBreakerDuration, localizer);

                policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

                return policy;
            }

            if (config.UseRetry)
                policy = PollyExtensions.RetryPolicy(config.RetryCount, config.RetryDelay, localizer);

            if (config.UseCircuitBreaker)
                policy = CircuitBreakerExtensions
                    .CircuitBreakerPolicy(config.CircuitBreakerFailuresAllowedBeforeBreaking, config.CircuitBreakerDuration, localizer);

            return policy;
        }
    }
}
