// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;

public class BankRegistration : EntityBase, ISupportsValidation
{
    /// <summary>
    ///     BankProfile used to specify bank configuration and apply transformations to external API (bank) requests.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required BankProfileEnum BankProfile { get; init; }

    /// <summary>
    ///     ID of software statement to use for registration. The ID must
    ///     correspond to a previously-added software statement.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required Guid SoftwareStatementId { get; set; }

    /// <summary>
    ///     Functional APIs specified in bank registration "scope".
    ///     If null, registration scope implied by software statement profile will be used.
    /// </summary>
    public RegistrationScopeEnum? RegistrationScope { get; init; }

    /// <summary>
    ///     Default fragment redirect URI to use for this registration. This URI must
    ///     be included in the redirect URIs used for this registration (these are specified by RedirectUris and if that is
    ///     null default to those specified in the software statement in software statement profile
    ///     SoftwareStatementProfileId).
    ///     If null, the default fragment redirect URI specified in the software statement profile
    ///     will be used.
    /// </summary>
    public string? DefaultFragmentRedirectUri { get; init; }

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
    public IList<string>? RedirectUris { get; init; }

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
    ///     Same as <see cref="ExternalApiSecret" /> but supplied via secrets.
    /// </summary>
    public SecretDescription? ExternalApiSecretFromSecrets { get; set; }

    /// <summary>
    ///     External (bank) API registration access token. Sometimes used to support registration adjustments etc. Only
    ///     relevant/used when <see cref="ExternalApiId" /> is non-null.
    /// </summary>
    public string? RegistrationAccessToken { get; set; }

    /// <summary>
    ///     Same as <see cref="RegistrationAccessToken" /> but supplied via secrets.
    /// </summary>
    public SecretDescription? RegistrationAccessTokenFromSecrets { get; set; }

    /// <summary>
    ///     Use simulated bank (only supported for some bank profiles).
    /// </summary>
    public bool UseSimulatedBank { get; init; }

    /// <summary>
    ///     Use v4 external (bank) API for AISP (when not specified default setting for bank profile is used).
    /// </summary>
    public bool? AispUseV4 { get; init; }

    /// <summary>
    ///     Use v4 external (bank) API for new PISP consent creation (when not specified default setting for bank profile is
    ///     used).
    /// </summary>
    public bool? PispUseV4 { get; init; }

    /// <summary>
    ///     Use v4 external (bank) API for new VRP consent creation (when not specified default setting for bank profile is
    ///     used).
    /// </summary>
    public bool? VrpUseV4 { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new BankRegistrationValidator()
            .ValidateAsync(this);
}
