// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

/// <summary>
///     Selected response headers from an external (bank) API call that are useful to thread through to callers,
///     e.g. for FAPI interaction tracing or observing the OB v4.0.1 rate-limit headers ("RateLimit" and
///     "RateLimit-Policy" - see
///     https://openbankinguk.github.io/read-write-api-site3/v4.0.1/profiles/read-write-data-api-profile.html#rate-limit-headers).
///     Rate-limit header values are passed through verbatim (unparsed) since their policy names and semantics are
///     ASPSP-specific and the headers may repeat, one per quota policy in effect - e.g. an ASPSP might return
///     RateLimit-Policy: "aisp-standard";q=325;w=5 alongside RateLimit: "aisp-standard";r=26;t=2.
/// </summary>
public record ExternalApiResponseHeaders
{
    public string? XFapiInteractionId { get; init; }

    public IReadOnlyList<string>? RateLimitPolicy { get; init; }

    public IReadOnlyList<string>? RateLimit { get; init; }
}
