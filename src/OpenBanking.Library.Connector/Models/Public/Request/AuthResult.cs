// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

public class AuthResult : ISupportsValidation
{
    /// <summary>
    ///     OpenID Connect response_mode used by redirect that supplied this result. This is
    ///     used to check result came by way of correct response_mode as security aspects
    ///     of response_modes differ.
    /// </summary>
    public OAuth2ResponseMode? ResponseMode { get; init; }

    /// <summary>
    ///     Redirect URL can optionally be supplied for checking.
    /// </summary>
    public string? RedirectUrl { get; init; }

    /// <summary>
    ///     App session ID can optionally be supplied for checking
    /// </summary>
    public string? AppSessionId { get; init; }

    public required OAuth2RedirectData RedirectData { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new AuthorisationRedirectObjectValidator()
            .ValidateAsync(this)!;
}
