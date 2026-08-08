// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

internal record ExternalApiHttpRequestIoError(
    string RequestHttpMethod,
    string RequestUrl,
    HttpRequestError HttpRequestError) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.ExternalApiHttpRequestIoError;
    public override string Title => "External API HTTP request I/O error";

    public override string Detail =>
        $"An HTTP request to an external API endpoint failed with I/O error of type '{HttpRequestError.ToString().ToCamelCase()}'.";

    public override int StatusCode => 502;

    public override IReadOnlyDictionary<string, object?> Extensions { get; } = new Dictionary<string, object?>
    {
        ["requestUrl"] = RequestUrl,
        ["requestHttpMethod"] = RequestHttpMethod,
        ["httpRequestError"] = HttpRequestError.ToString().ToCamelCase()
    };
}

internal record ExternalApiHttpRequestTimeout(
    string RequestHttpMethod,
    string RequestUrl) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.ExternalApiHttpRequestTimeout;
    public override string Title => "External API HTTP request time-out";
    public override string Detail => "An HTTP request to an external API endpoint timed out.";
    public override int StatusCode => 504;

    public override IReadOnlyDictionary<string, object?> Extensions { get; } = new Dictionary<string, object?>
    {
        ["requestUrl"] = RequestUrl,
        ["requestHttpMethod"] = RequestHttpMethod
    };
}

internal record ExternalApiHttpRequestFailure(
    string RequestHttpMethod,
    string RequestUrl,
    int ResponseStatusCode,
    object ParsedResponseBody,
    string? XFapiInteractionId,
    int? RetryAfterSeconds,
    IReadOnlyList<string>? RateLimitPolicy,
    IReadOnlyList<string>? RateLimit) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.ExternalApiHttpRequestFailure;
    public override string Title => "External API HTTP request failure";

    public override string Detail =>
        $"An HTTP request to an external API endpoint returned an error response with HTTP status code {ResponseStatusCode}.";

    public override int StatusCode => 502;

    public override IReadOnlyDictionary<string, object?> Extensions
    {
        get
        {
            var extensions = new Dictionary<string, object?>
            {
                ["requestUrl"] = RequestUrl,
                ["requestHttpMethod"] = RequestHttpMethod,
                ["responseStatusCode"] = ResponseStatusCode,
                ["responseBody"] = ParsedResponseBody
            };
            if (XFapiInteractionId is not null)
            {
                extensions["xFapiInteractionId"] = XFapiInteractionId;
            }
            if (RetryAfterSeconds is not null)
            {
                extensions["retryAfterSeconds"] = RetryAfterSeconds;
            }
            if (RateLimitPolicy is not null)
            {
                extensions["rateLimitPolicy"] = RateLimitPolicy;
            }
            if (RateLimit is not null)
            {
                extensions["rateLimit"] = RateLimit;
            }
            return extensions;
        }
    }
}
