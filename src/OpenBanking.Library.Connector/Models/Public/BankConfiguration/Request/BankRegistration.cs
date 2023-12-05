// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

public class BankRegistration : Base, ISupportsValidation
{
    /// <summary>
    ///     BankProfile used to specify bank configuration and apply transformations to external API (bank) requests.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public BankProfileEnum BankProfile { get; set; }

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
    ///     Functional APIs specified in bank registration "scope".
    ///     If null, registration scope implied by software statement profile will be used.
    /// </summary>
    public RegistrationScopeEnum? RegistrationScope { get; set; }

    /// <summary>
    ///     Default fragment redirect URI to use for this registration. This URI must
    ///     be included in the redirect URIs used for this registration (these are specified by RedirectUris and if that is
    ///     null default to those specified in the software statement in software statement profile
    ///     SoftwareStatementProfileId).
    ///     If null, the default fragment redirect URI specified in the software statement profile
    ///     will be used.
    /// </summary>
    public string? DefaultFragmentRedirectUri { get; set; }

    /// <summary>
    ///     Default query redirect URI to use for this registration. This URI must
    ///     be included in the redirect URIs used for this registration (these are specified by RedirectUris and if that is
    ///     null default to those specified in the software statement in software statement profile
    ///     SoftwareStatementProfileId).
    ///     If null, the default query redirect URI specified in the software statement profile
    ///     will be used.
    /// </summary>
    public string? DefaultQueryRedirectUri { get; set; }

    /// <summary>
    ///     Redirect URIs to use for this registration. Must be a subset of those specified in
    ///     the software statement in software statement profile SoftwareStatementProfileId.
    ///     If null, redirect URIs specified in the software statement will be used.
    /// </summary>
    public IList<string>? RedirectUris { get; set; }

    /// <summary>
    ///     External (bank) API ID, i.e. ID of OAuth2 client at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    ///     Use this to supply information about a previously-created external API (bank) registration (OAuth2 client) created
    ///     at the bank either
    ///     via API or the bank's developer portal.
    ///     When non-null, this external registration will re-used and Dynamic Client Registration (DCR) will not be performed.
    /// </summary>
    public string? ExternalApiId { get; set; }

    /// <summary>
    ///     External (bank) API secret. Present to allow use of legacy token auth method "client_secret_basic" in sandboxes
    ///     etc. Only relevant/used when <see cref="ExternalApiId" /> is non-null.
    /// </summary>
    public string? ExternalApiSecret { get; set; }

    /// <summary>
    ///     External (bank) API registration access token. Sometimes used to support registration adjustments etc. Only
    ///     relevant/used when <see cref="ExternalApiId" /> is non-null.
    /// </summary>
    public string? RegistrationAccessToken { get; set; }

    /// <summary>
    ///     Use simulated bank (only supported for some bank profiles).
    /// </summary>
    public bool UseSimulatedBank { get; set; } = false;

    public async Task<ValidationResult> ValidateAsync() =>
        await new BankRegistrationValidator()
            .ValidateAsync(this)!;
}
