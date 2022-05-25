// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request
{
    /// <summary>
    ///     A Bank is the base type used to describe a bank ("ASPSP") in Open Banking Connector.
    ///     It effectively captures the static data associated with a bank which is then used when
    ///     creating bank registrations and APIs for that bank.
    ///     Each <see cref="BankRegistration" /> is
    ///     a child object of a Bank.
    /// </summary>
    public class Bank : Base, ISupportsValidation
    {
        public BankProfileEnum? BankProfile { get; set; }

        /// <summary>
        ///     JWK Set URI (normally supplied from OpenID Configuration)
        /// </summary>
        public string? JwksUri { get; set; }

        public OAuth2ResponseMode DefaultResponseMode { get; set; } = OAuth2ResponseMode.Fragment;

        public bool SupportsSca { get; set; } = false;

        /// <summary>
        ///     Issuer URL to use when creating Bank Registration which indicates the presence of valid
        ///     OpenID Provider Configuration at the endpoint GET "/{IssuerUrl}/.well-known/openid-configuration".
        ///     If no such OpenID Provider Configuration is available, please set this value to null.
        /// </summary>
        public string? IssuerUrl { get; set; }

        /// <summary>
        ///     FAPI financial ID to use when creating Bank Registration
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public string FinancialId { get; set; } = null!;


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
        ///     Version of Open Banking Dynamic Client Registration API to use
        ///     for bank registration.
        /// </summary>
        public DynamicClientRegistrationApiVersion DynamicClientRegistrationApiVersion { get; set; } =
            DynamicClientRegistrationApiVersion.Version3p3;

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     For a well-behaved bank, normally this object should be null.
        /// </summary>
        public CustomBehaviourClass? CustomBehaviour { get; set; }


        public async Task<ValidationResult> ValidateAsync() =>
            await new BankValidator()
                .ValidateAsync(this)!;
    }
}
