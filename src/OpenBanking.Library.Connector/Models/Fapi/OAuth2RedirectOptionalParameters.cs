// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

/// <summary>
///     Auth response supplied by redirect according to response_mode.
///     We call this <see cref="OAuth2RedirectOptionalParameters" /> to avoid using "response" word in Fluent request
///     class.
/// </summary>
public class OAuth2RedirectOptionalParameters : ISupportsValidation
{
    public string? Error { get; init; }

    public string? IdToken { get; init; }

    public string? Code { get; init; }

    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}
