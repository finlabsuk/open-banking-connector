// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public delegate ConcurrentDictionary<string, string> GetExtraParameters(bool reAuthNotInitialAuth);

public class ConsentAuthGetCustomBehaviour
{
    // Set prefix for consent ID claim(s)
    public string? ConsentIdClaimPrefix { get; set; }

    // Override aud claim
    public string? AudClaim { get; set; }

    //public bool? AddRedundantOAuth2RedirectUriRequestParameter { get; set; }

    public string? Scope { get; init; }

    public GetExtraParameters? GetExtraParameters { get; init; }

    /// <summary>
    ///     Use consent to check for re-auth case (may be necessary if Open Bnaking Connector
    ///     does not know about previous successful auth)
    /// </summary>
    public bool? DetectReAuthCaseViaConsentStatus { get; init; }

    public string? ExtraConsentParameterName { get; init; }

    public bool? DoNotUseUrlPathEncoding { get; init; }

    public string? SingleBase64EncodedParameterName { get; init; }

    public bool? AddRedundantOAuth2StateRequestParameter { get; set; }

    public bool? AddRedundantOAuth2NonceRequestParameter { get; set; }

    public bool? UsePkce { get; set; }

    public IdTokenProcessingCustomBehaviour? IdTokenProcessingCustomBehaviour { get; set; }
}
