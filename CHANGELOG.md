# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2026-05-21

### ADDED
- Added XML documentation summaries in English to all public classes and methods for improved IntelliSense support.

### CHANGED
- Renamed internal method `HandleResponse` to `HandleResponseAsync` in `ServiceClient` to follow the `Async` suffix convention.
- Renamed internal method `Deserialize` to `DeserializeAsync` in `ServiceClient` to follow the `Async` suffix convention.
- Changed `DeserializeAsync` return type from `Task<T>` to `Task<T?>` and added `where T : class` constraint to safely handle null deserialization results.
- **BREAKING**: Made `ResponseDto<T>.Data` property nullable (`T?`). Consumers must check `Success == true` before accessing `Data`; otherwise `Data` will be `null`.

## [2.1.0] - 2026-01-09

### CHANGED
- Updated internal access modifiers for better encapsulation.
- General code cleanup and formatting improvements.

## [2.0.0] - 2025-09-12

### ADDED
- Added configurable `HttpClientPolicyConfiguration` for Retry, Circuit Breaker, and Timeout policies.
- Added extension methods `RetryExtensions`, `CircuitBreakerExtensions`, and `TimeoutExtensions` for Polly policies.
- Added support for localized error messages using `IStringLocalizer`.

### CHANGED
- Refactored dependency injection registration with `AddHttpClientService`.
- Improved error response standardization and status code handling.

### FIXED
- Fixed generic bad request responses handling.
- Fixed localization resource path configuration.
- Fixed internal string localizer registration logic.

## [1.0.0] - 2025-08-11

### ADDED
- Initial stable release.
- Added `IServiceClient` with GET, POST, PUT, and DELETE methods.
- Added typed and non-typed response support via `ResponseDto<T>` and `BaseResponseDto`.
- Added `HttpContentExtensions.GenerateStringContent` helper.
- Added Polly-based resilience policies (Retry and Circuit Breaker).
- Added dedicated circuit breaker policy for HTTP 429 (Too Many Requests).
- Added support for request headers configuration.

### CHANGED
- Removed `DefaultRequestHeaders` approach; refined named client configuration.

### FIXED
- Fixed icon URL in package metadata.
