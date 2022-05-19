// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
    public class BankRegistration : Base, ISupportsValidation
    {
        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public Guid BankId { get; set; }

        /// <summary>
        ///     ID of software statement profile used to create bank registration. Only
        ///     IDs which have been specified via configuration
        ///     will be accepted.
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
        ///     Version of Open Banking Dynamic Client Registration API to use
        ///     for bank registration.
        /// </summary>
        public DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; set; } =
            DynamicClientRegistrationApiVersion.Version3p3;

        /// <summary>
        ///     Functional APIs used for bank registration.
        ///     If not supplied, registration scope implied by software statement profile will be used.
        /// </summary>
        public RegistrationScopeEnum? RegistrationScope { get; set; }

        /// <summary>
        ///     Issuer URL to use when creating Bank Registration which indicates the presence of valid
        ///     OpenID Provider Configuration at the endpoint GET "/{IssuerUrl}/.well-known/openid-configuration".
        ///     If no such OpenID Provider Configuration is available, please set this value to null.
        /// </summary>
        public string? IssuerUrl { get; set; } = null!;

        /// <summary>
        ///     Registration endpoint. Specify null to set this from Issuer URL OpenID Provider Configuration.
        ///     Only used by operations that access bank registration endpoint(s), e.g. DCR. If DCR and optional GET, PUT, DELETE
        ///     endpoints for bank registration are not supported, this value will not be used.
        /// </summary>
        public string? RegistrationEndpoint { get; set; }

        /// <summary>
        ///     Token endpoint. Specify null to set this from Issuer URL OpenID Provider Configuration.
        /// </summary>
        public string? TokenEndpoint { get; set; }

        /// <summary>
        ///     Authorization endpoint. Specify null to set this from Issuer URL OpenID Provider Configuration.
        /// </summary>
        public string? AuthorizationEndpoint { get; set; }

        /// <summary>
        ///     Token endpoint authorisation method. Specify null for "most preferred" method to be selected based on
        ///     supported methods in Issuer URL OpenID Provider Configuration.
        /// </summary>
        public TokenEndpointAuthMethod? TokenEndpointAuthMethod { get; set; }

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     For a well-behaved bank, normally this object should be null.
        /// </summary>
        public CustomBehaviourClass? CustomBehaviour { get; set; }


        /// <summary>
        ///     Existing bank registration (OAuth2 client) information. When non-null, this will be used instead of
        ///     creating a new registration at bank via DCR.
        /// </summary>
        public ExternalApiObject? ExternalApiObject { get; set; }

        /// <summary>
        ///     If registration already exists for bank, allow creation of additional one. NB this may
        ///     disrupt existing registration depending on bank support for multiple registrations.
        /// </summary>
        public bool AllowMultipleRegistrations { get; set; } = true;

        public async Task<ValidationResult> ValidateAsync() =>
            await new BankRegistrationValidator()
                .ValidateAsync(this)!;
    }
}
