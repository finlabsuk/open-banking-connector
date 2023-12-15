// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

public class IdTokenProcessingCustomBehaviour
{
    public bool? DoNotValidateIdToken { get; set; }

    public bool? IdTokenNonceClaimIsPreviousValue { get; set; }

    public bool? IdTokenMayNotHaveAcrClaim { get; set; }

    public bool? DoNotValidateIdTokenAcrClaim { get; set; }

    public bool? IdTokenMayNotHaveAuthTimeClaim { get; set; }

    public bool? IdTokenMayNotHaveConsentIdClaim { get; set; }

    public DateTimeOffsetUnixConverterEnum? IdTokenExpirationTimeClaimJsonConverter { get; set; }
}
