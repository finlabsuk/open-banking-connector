// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;

public class IdTokenProcessingCustomBehaviour
{
    /// <summary>
    ///     ID token "sub" claim type.
    /// </summary>
    public IdTokenSubClaimType? IdTokenSubClaimType { get; set; }

    public bool? DoNotValidateIdToken { get; set; }

    public bool? IdTokenNonceClaimIsPreviousValue { get; set; }

    public bool? IdTokenMayNotHaveAcrClaim { get; set; }

    public bool? DoNotValidateIdTokenAcrClaim { get; set; }

    public bool? IdTokenMayNotHaveAuthTimeClaim { get; set; }

    public bool? IdTokenMayNotHaveConsentIdClaim { get; set; }

    public bool? IdTokenMayNotHaveNonceClaim { get; set; }

    public string? IssClaim { get; set; }

    public DateTimeOffsetUnixConverterEnum? IdTokenExpirationTimeClaimJsonConverter { get; set; }

    public static bool GetDoNotValidateIdToken(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.DoNotValidateIdToken ?? baseCustomBehaviour?.DoNotValidateIdToken ?? false;

    public static bool GetIdTokenNonceClaimIsPreviousValue(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IdTokenNonceClaimIsPreviousValue ??
        baseCustomBehaviour?.IdTokenNonceClaimIsPreviousValue ?? false;

    public static IdTokenSubClaimType GetIdTokenSubClaimType(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IdTokenSubClaimType ?? baseCustomBehaviour?.IdTokenSubClaimType
        ?? Models.Public.Management.IdTokenSubClaimType.ConsentId;

    public static bool GetIdTokenMayNotHaveAcrClaim(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IdTokenMayNotHaveAcrClaim ?? baseCustomBehaviour?.IdTokenMayNotHaveAcrClaim ?? false;

    public static bool GetDoNotValidateIdTokenAcrClaim(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.DoNotValidateIdTokenAcrClaim ?? baseCustomBehaviour?.DoNotValidateIdTokenAcrClaim ?? false;

    public static bool GetIdTokenMayNotHaveAuthTimeClaim(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IdTokenMayNotHaveAuthTimeClaim ?? baseCustomBehaviour?.IdTokenMayNotHaveAuthTimeClaim ?? false;

    public static bool GetIdTokenMayNotHaveConsentIdClaim(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IdTokenMayNotHaveConsentIdClaim ??
        baseCustomBehaviour?.IdTokenMayNotHaveConsentIdClaim ?? false;

    public static bool GetIdTokenMayNotHaveNonceClaim(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IdTokenMayNotHaveNonceClaim ?? baseCustomBehaviour?.IdTokenMayNotHaveNonceClaim ?? false;

    public static string? GetIssClaim(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IssClaim ?? baseCustomBehaviour?.IssClaim;

    public static DateTimeOffsetUnixConverterEnum? GetIdTokenExpirationTimeClaimJsonConverter(
        IdTokenProcessingCustomBehaviour? customBehaviour,
        IdTokenProcessingCustomBehaviour? baseCustomBehaviour) =>
        customBehaviour?.IdTokenExpirationTimeClaimJsonConverter ??
        baseCustomBehaviour?.IdTokenExpirationTimeClaimJsonConverter;
}
