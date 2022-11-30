// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation.Results;

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
        /// <summary>
        ///     BankProfile used to supply default values for unspecified properties and apply transformations to external API
        ///     requests. Use null to not specify a bank profile.
        /// </summary>
        public BankProfileEnum? BankProfile { get; set; }

        /// <summary>
        ///     JWK Set URI. Normally null which means value obtained from OpenID Configuration (IssuerUrl).
        /// </summary>
        public string? JwksUri { get; set; }

        /// <summary>
        ///     Supports Secure Customer Authentication. Normally set to false for sandboxes.
        ///     Normally null which means value obtained from BankProfile.
        /// </summary>
        public bool? SupportsSca { get; set; }

        /// <summary>
        ///     Issuer URL which identifies bank in OAuth2/OpenID Connect. It also generally implies existence of valid
        ///     OpenID Provider Configuration at the endpoint GET "/{IssuerUrl}/.well-known/openid-configuration".
        ///     Normally null which means value obtained from BankProfile.
        /// </summary>
        public string? IssuerUrl { get; set; }

        /// <summary>
        ///     FAPI financial ID (org ID) of bank.
        ///     Normally null which means value obtained from BankProfile.
        /// </summary>
        public string? FinancialId { get; set; }

        /// <summary>
        ///     Registration endpoint. Normally null which means value supplied by OpenID Provider Configuration (IssuerUrl) if
        ///     available.
        ///     Used by operations that access bank registration endpoint(s), i.e. DCR and optional GET, PUT, DELETE
        ///     endpoints for bank registration.
        /// </summary>
        public string? RegistrationEndpoint { get; set; }

        /// <summary>
        ///     Allow registration endpoint to set to null if none supplied and none available via OpenID Configuration
        ///     (IssuerUrl).
        ///     This is to allow for banks which do not support DCR.
        /// </summary>
        public bool AllowNullRegistrationEndpoint { get; set; } = false;

        /// <summary>
        ///     Token endpoint. Normally null which means value obtained from OpenID Configuration (IssuerUrl).
        /// </summary>
        public string? TokenEndpoint { get; set; }

        /// <summary>
        ///     Authorization endpoint. Normally null which means value obtained from OpenID Configuration (IssuerUrl).
        /// </summary>
        public string? AuthorizationEndpoint { get; set; }

        /// <summary>
        ///     Version of Open Banking Dynamic Client Registration API to use
        ///     for bank registration.  Normally null which means value obtained from BankProfile.
        /// </summary>
        public DynamicClientRegistrationApiVersion? DynamicClientRegistrationApiVersion { get; set; }

        /// <summary>
        ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
        ///     Normally null which means determined value obtained from BankProfile if available or else kept null.
        ///     For a well-behaved bank, the determined value should be null - but this is often not the case for
        ///     sandboxes unfortunately.
        /// </summary>
        public CustomBehaviourClass? CustomBehaviour { get; set; }


        public async Task<ValidationResult> ValidateAsync() =>
            await new BankValidator()
                .ValidateAsync(this)!;
    }
}
