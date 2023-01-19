// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

public class ConsentAuthGetCustomBehaviour
{
    // Set prefix for consent ID claim(s)
    public string? ConsentIdClaimPrefix { get; set; }

    // Override aud claim
    public string? AudClaim { get; set; }

    public bool? DoNotValidateIdToken { get; set; }

    public Acr? IdTokenAcrClaim { get; set; }

    public bool? IdTokenNonceClaimIsPreviousValue { get; set; }
}
