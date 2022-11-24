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

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
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
        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; set; }

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
        ///     If registration already exists for bank, allow creation of an additional one. NB this may
        ///     disrupt existing registration depending on bank support for multiple registrations.
        /// </summary>
        public bool AllowMultipleRegistrations { get; set; } = false;

        public async Task<ValidationResult> ValidateAsync() =>
            await new BankRegistrationValidator()
                .ValidateAsync(this)!;
    }
}
