using IATec.Shared.HttpClient.Configurations;
using IATec.Shared.HttpClient.Extensions;
using IATec.Shared.HttpClient.Resources;
using IATec.Shared.HttpClient.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.Registry;
using System;
using System.Linq;
using System.Net.Http;

namespace IATec.Shared.HttpClient.DependencyInjection
{
    public static class HttpClientDependencyInjection
    {
        const string PolicyKeyPrefix = "iatec:httpclient:";
        const string DefaultClientName = "DefaultHttpClient";

        public static IServiceCollection AddHttpClientService(
            this IServiceCollection services,
            Action<HttpClientPolicyConfiguration> configurePolicy,
            Action<System.Net.Http.HttpClient>? configureClient = null,
            string clientName = DefaultClientName)
        {
            if (services.All(sd => sd.ServiceType != typeof(IStringLocalizerFactory)))
                services.AddLocalization(options => options.ResourcesPath = "");

            IPolicyRegistry<string> registry = services.AddPolicyRegistry();

            var config = new HttpClientPolicyConfiguration();
            configurePolicy?.Invoke(config);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var localizer = serviceProvider.GetRequiredService<IStringLocalizer<Messages>>();

                var combinedPolicy = GetCombinedPolicy(config, localizer);

                string policyKey = $"{PolicyKeyPrefix}:{clientName}";

                if (registry.ContainsKey(policyKey))
                    registry[policyKey] = combinedPolicy;
                else
                    registry.Add(policyKey, combinedPolicy);

                var builder = services.AddHttpClient(clientName, configureClient ?? (_ => { }));
                builder.AddPolicyHandlerFromRegistry(policyKey);

                bool isLegacy = (configureClient is null) && string.Equals(clientName, DefaultClientName, StringComparison.Ordinal);

                if (isLegacy)
                {
                    services.TryAddTransient<IServiceClient>(serviceProvider =>
                    {
                        var http = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(clientName);
                        return new ServiceClient(http, localizer);
                    });
                }
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