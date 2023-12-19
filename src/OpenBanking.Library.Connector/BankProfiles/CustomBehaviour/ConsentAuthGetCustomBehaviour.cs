// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public class ConsentAuthGetCustomBehaviour
{
    // Set prefix for consent ID claim(s)
    public string? ConsentIdClaimPrefix { get; set; }

    // Override aud claim
    public string? AudClaim { get; set; }

    //public bool? AddRedundantOAuth2RedirectUriRequestParameter { get; set; }

    public bool? AddRedundantOAuth2StateRequestParameter { get; set; }

    public bool? AddRedundantOAuth2NonceRequestParameter { get; set; }

    public IdTokenProcessingCustomBehaviour? IdTokenProcessingCustomBehaviour { get; set; }
}
