// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public;

public class ExternalApiResponseInfo
{
    public required string? XFapiInteractionId { get; init; }

    /// <summary>
    ///     Raw value(s) of the optional OB v4.0.1 "RateLimit-Policy" response header, if returned by the ASPSP,
    ///     e.g. "aisp-standard";q=325;w=5. One entry per quota policy in effect; unparsed, since policy names and
    ///     semantics are ASPSP-specific.
    ///     See
    ///     https://openbankinguk.github.io/read-write-api-site3/v4.0.1/profiles/read-write-data-api-profile.html#rate-limit-headers
    /// </summary>
    public IReadOnlyList<string>? RateLimitPolicy { get; init; }

    /// <summary>
    ///     Raw value(s) of the optional OB v4.0.1 "RateLimit" response header, if returned by the ASPSP,
    ///     e.g. "aisp-standard";r=26;t=2. One entry per quota policy in effect; unparsed, since policy names and
    ///     semantics are ASPSP-specific.
    ///     See
    ///     https://openbankinguk.github.io/read-write-api-site3/v4.0.1/profiles/read-write-data-api-profile.html#rate-limit-headers
    /// </summary>
    public IReadOnlyList<string>? RateLimit { get; init; }
}
