# IATec.Shared.Net.HttpClient

A shared .NET Standard 2.1 library that provides a resilient and easy-to-use HTTP service client for IATec internal projects. It wraps `System.Net.Http.HttpClient` with Polly-based retry, circuit breaker, and timeout policies, along with standardized error responses.

## Installation

The package is published as a NuGet package with the ID `IATec.Shared.HttpClient`.

```bash
dotnet add package IATec.Shared.HttpClient
```

## Features

- 🌐 Named `HttpClient` factory support
- 🔄 Retry policy with configurable attempts and delays
- ⚡ Circuit breaker with dedicated handling for HTTP 429 (Too Many Requests)
- ⏱ Optional timeout policy
- 📦 Standardized response DTOs (`ResponseDto<T>`, `BaseResponseDto`)
- 🌍 Localized error messages (supports `pt-BR`, `es`, and more via resx)
- 🔧 Simple dependency injection setup

## Usage

### 1. Register the service

In your `Startup.cs` or `Program.cs`:

```csharp
using IATec.Shared.HttpClient.Configurations;
using IATec.Shared.HttpClient.DependencyInjection;

services.AddHttpClientService(
    configurePolicy: policy =>
    {
        policy.UseRetry = true;
        policy.RetryCount = 3;
        policy.RetryDelay = TimeSpan.FromSeconds(2);
        policy.UseCircuitBreaker = true;
        policy.CircuitBreakerFailuresAllowedBeforeBreaking = 4;
        policy.CircuitBreakerDuration = TimeSpan.FromSeconds(30);
        policy.RequestTimeout = TimeSpan.FromSeconds(60);
    },
    configureClient: client =>
    {
        client.BaseAddress = new Uri("https://api.example.com");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    },
    clientName: "MyApiClient"
);
```

### 2. Inject and use

```csharp
using IATec.Shared.HttpClient.Service;

public class MyService
{
    private readonly IServiceClient _client;

    public MyService(IServiceClientFactory factory)
    {
        _client = factory.Create("MyApiClient");
    }

    public async Task DoWorkAsync()
    {
        var response = await _client.GetAsync<MyModel>("/data");

        if (response.Success)
        {
            var data = response.Data;
            // use data...
        }
        else
        {
            foreach (var error in response.Errors)
            {
                Console.WriteLine($"[{error.StatusCode}] {error.Message}");
            }
        }
    }
}
```

### 3. Creating HTTP content

You can use the built-in extension to serialize objects into `StringContent`:

```csharp
using IATec.Shared.HttpClient.Extensions;

var content = myObject.GenerateStringContent("application/json");
var response = await _client.PostAsync<MyResponse>("/endpoint", content);
```

## Response DTOs

- **`BaseResponseDto`** – Contains `Success`, `Errors`
- **`ResponseDto<T>`** – Inherits from `BaseResponseDto` and adds `Data`
- **`ErrorDto`** – Contains `StatusCode` and `Message`

## Resiliency Policies

| Policy | Description |
|--------|-------------|
| **Retry** | Retries on `HttpRequestException`, 5xx, and 408 responses |
| **Circuit Breaker** | Opens the circuit after N failures and stays open for a configured duration |
| **Too Many Requests** | Dedicated circuit breaker for HTTP 429 responses |
| **Timeout** | Optional request timeout using Polly timeout strategy |

## Changelog

See [CHANGELOG.md](./CHANGELOG.md) for a detailed history of changes.
