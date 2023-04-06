﻿// Licensed to Finnovation Labs Limited under one or more agreements.
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
    ///     JWK Set URI. Normally null which means value obtained from OpenID Configuration (IssuerUrl).
    /// </summary>
    public string? JwksUri { get; set; }

    /// <summary>
    ///     Registration endpoint. Normally null which means value supplied by OpenID Provider Configuration (IssuerUrl) if
    ///     available.
    ///     Used by operations that access bank registration endpoint(s), i.e. DCR and optional GET, PUT, DELETE
    ///     endpoints for bank registration.
    /// </summary>
    public string? RegistrationEndpoint { get; set; }

    /// <summary>
    ///     Token endpoint. Normally null which means value obtained from OpenID Configuration (IssuerUrl).
    /// </summary>
    public string? TokenEndpoint { get; set; }

    /// <summary>
    ///     Authorization endpoint. Normally null which means value obtained from OpenID Configuration (IssuerUrl).
    /// </summary>
    public string? AuthorizationEndpoint { get; set; }

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
    ///     Forces BankRegistrationGroup to be null (i.e. no BankRegistrationGroup) rather than the value automatically
    ///     determined by the BankProfile and RegistrationScope.
    ///     For each BankRegistrationGroup, the same external API (bank)
    ///     registration is
    ///     re-used and DCR will only be performed the first time the BankRegistrationGroup is used.
    ///     This is to prevent unnecessary duplicate
    ///     external API (bank) registrations which may disrupt/overwrite an
    ///     existing such registration depending on bank behaviour.
    ///     The safeguard of the auto-determined BankRegistrationGroup can be removed, and the use of DCR forced, when
    ///     ForceNullBankRegistrationGroup is true. Please use this setting with care, and it is suggested to delete any
    ///     unwanted external API
    ///     registrations at the bank possibly via a support ticket if API support for this is not provided.
    /// </summary>
    public bool ForceNullBankRegistrationGroup { get; set; } = false;

    public async Task<ValidationResult> ValidateAsync() =>
        await new BankRegistrationValidator()
            .ValidateAsync(this)!;
}
