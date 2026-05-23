// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal enum IdTokenValidationErrorType
{
    IdTokenExpired,
    ConsentIdMissing,
    ConsentIdMismatch,
    NonceMissing,
    NonceMismatch,
    AuthTimeMissing,
    AcrMissing,
    AcrMismatch,
    IssuerMismatch,
    AudienceMismatch,
    SubjectEmpty,
    SubjectEndUserIdNotFound,
    SubjectEndUserIdMismatch,
    SubjectConsentIdMismatch,
    SubjectClientIdMismatch,
    CodeHashMismatch,
    StateHashMismatch,
    AccessTokenHashMismatch
}

internal record IdTokenValidationError(
    IdTokenBase IdToken,
    IdTokenValidationErrorType IdTokenValidationErrorType) : ServerError
{
    public override ServerErrorType ServerErrorType => ServerErrorType.IdTokenValidationError;
    public override string Title => "ID token validation error";

    public override string Detail => IdTokenValidationErrorType switch
    {
        IdTokenValidationErrorType.IdTokenExpired => $"ID token has expired (exp: {IdToken.Exp:u}).",
        IdTokenValidationErrorType.ConsentIdMissing => "Consent ID not provided in ID token.",
        IdTokenValidationErrorType.ConsentIdMismatch => "Consent ID from ID token does not match expected value.",
        IdTokenValidationErrorType.NonceMissing => "Nonce not provided in ID token.",
        IdTokenValidationErrorType.NonceMismatch => "Nonce from ID token does not match expected value.",
        IdTokenValidationErrorType.AuthTimeMissing => "Auth time not provided in ID token.",
        IdTokenValidationErrorType.AcrMissing => "ACR not provided in ID token.",
        IdTokenValidationErrorType.AcrMismatch => "ACR from ID token does not match expected value.",
        IdTokenValidationErrorType.IssuerMismatch => "Issuer from ID token does not match expected value.",
        IdTokenValidationErrorType.AudienceMismatch => "Audience from ID token does not match expected value.",
        IdTokenValidationErrorType.SubjectEmpty => "Subject from ID token empty.",
        IdTokenValidationErrorType.SubjectEndUserIdNotFound => "No end user ID found for use in ID token validation.",
        IdTokenValidationErrorType.SubjectEndUserIdMismatch =>
            "Subject from ID token does not match expected end user ID.",
        IdTokenValidationErrorType.SubjectConsentIdMismatch =>
            "Subject from ID token does not match expected consent ID.",
        IdTokenValidationErrorType.SubjectClientIdMismatch =>
            "Subject from ID token does not match expected client ID.",
        IdTokenValidationErrorType.CodeHashMismatch => "Code hash from ID token does not match code.",
        IdTokenValidationErrorType.StateHashMismatch => "State hash from ID token does not match state.",
        IdTokenValidationErrorType.AccessTokenHashMismatch =>
            "Access token hash from ID token does not match access token.",
        _ => throw new ArgumentOutOfRangeException()
    };

    public override int StatusCode => 502;

    public override IReadOnlyDictionary<string, object?> Extensions { get; } =
        new Dictionary<string, object?>
        {
            ["iss"] = IdToken.Issuer,
            ["iat"] = IdToken.Iat,
            ["exp"] = IdToken.Exp,
            ["acr"] = IdToken.Acr?.ToString().ToCamelCase(),
            ["authTime"] = IdToken.AuthTime,
            ["idTokenValidationErrorType"] = IdTokenValidationErrorType.ToString().ToCamelCase()
        };
}
