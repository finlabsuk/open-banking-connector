// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

public class BankRegistration : Base, ISupportsValidation
{
    /// <summary>
    ///     BankProfile used to supply default values for unspecified properties and apply transformations to external API
    ///     requests. Use null to not specify a bank profile.
    /// </summary>
    public BankProfileEnum? BankProfile { get; set; }

    /// <summary>
    ///     Target bank for registration.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public Guid BankId { get; set; }

    /// <summary>
    ///     ID of software statement profile to use for registration. The ID must
    ///     correspond to a software statement profile provided via secrets/configuration.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public string SoftwareStatementProfileId { get; set; } = null!;

    /// <summary>
    ///     Optional override case to use with software statement and certificate profiles. Override cases
    ///     can be used for bank-specific customisations to profiles, e.g. different transport certificate DN string.
    ///     When null no override case is specified.
    /// </summary>
    public string? SoftwareStatementProfileOverrideCase { get; set; }

    /// <summary>
    ///     Token endpoint authorisation method.
    ///     Specify null to use value from BankProfile.
    /// </summary>
    public TokenEndpointAuthMethod? TokenEndpointAuthMethod { get; set; }

    /// <summary>
    ///     Functional APIs specified in bank registration "scope".
    ///     If null, registration scope implied by software statement profile will be used.
    /// </summary>
    public RegistrationScopeEnum? RegistrationScope { get; set; }

    /// <summary>
    ///     Default response mode for OpenID auth request.
    ///     Normally null which means value obtained from BankProfile.
    /// </summary>
    public OAuth2ResponseMode? DefaultResponseMode { get; set; }

    /// <summary>
    ///     Default redirect URI to use for this registration. This redirect URI must
    ///     be included in the software statement in software statement profile SoftwareStatementProfileId.
    ///     If null, the default redirect URI specified in the software statement profile will be used.
    /// </summary>
    public string? DefaultRedirectUri { get; set; }

    /// <summary>
    ///     Other redirect URIs in addition to default one to use for this registration.
    ///     Each redirect URI must
    ///     be included in the software statement in software statement profile SoftwareStatementProfileId.
    ///     If null, redirect URIs in the software statement profile (excluding that used as the default) will be used.
    /// </summary>
    public List<string>? OtherRedirectUris { get; set; }

    /// <summary>
    ///     Information about a previously-created BankRegistration (OAuth2 client) created at the external (bank) API.
    ///     When non-null, this will be referenced in the local object
    ///     instead of
    ///     creating a new external BankRegistration object at the external API via DCR.
    /// </summary>
    public ExternalApiBankRegistration? ExternalApiObject { get; set; }

    /// <summary>
    ///     Forces BankRegistrationGroup to be null regardless of what it is set to. This can be used to force
    ///     a null value when the BankProfile is null or to ignore the BankProfile value.
    /// </summary>
    public bool ForceNullBankRegistrationGroup { get; set; } = false;

    /// <summary>
    ///     Bank registration group. When specified, for a given SoftwareStatementProfileId, the same external API (bank)
    ///     registration object is
    ///     re-used by all members of the group and DCR is only performed if required the first time the group is specified.
    ///     This can be used to prevent multiple
    ///     registrations which may disrupt an
    ///     existing registration depending on bank support for multiple registrations.
    ///     If null, the value will be obtained from the bank profile.
    ///     Regardless of setting, this value will be forced to be null when ForceNullBankRegistrationGroup is
    ///     true.
    /// </summary>
    public BankRegistrationGroup? BankRegistrationGroup { get; set; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new BankRegistrationValidator()
            .ValidateAsync(this)!;
}
