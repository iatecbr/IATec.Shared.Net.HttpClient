using IATec.Shared.HttpClient.Resources;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IATec.Shared.HttpClient.Extensions
{
    public static class TimeoutExtensions
    {
        public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy(
            TimeSpan timeout,
            IStringLocalizer<Messages> localizer
            )
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                timeout,
                TimeoutStrategy.Optimistic,
                onTimeoutAsync: async (context, span, _, ex) =>
                {
                    Console.WriteLine(localizer
                            .GetString(nameof(Messages.TimeoutMessage)));

                    await Task.CompletedTask;
                });
        }
    }
}