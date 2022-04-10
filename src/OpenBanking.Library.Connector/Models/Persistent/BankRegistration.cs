// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using BankRegistrationRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;
using OAuth2RequestObjectClaimsOverridesRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.OAuth2RequestObjectClaimsOverrides;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class BankRegistration :
        BaseEntity,
        IBankRegistrationPublicQuery
    {
        public BankRegistration(
            string? name,
            string? reference,
            Guid id,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string softwareStatementProfileId,
            string? softwareStatementAndCertificateProfileOverrideCase,
            RegistrationScopeEnum registrationScope,
            ClientRegistrationApiVersion clientRegistrationApi,
            OpenIdConfiguration openIdConfigurationResponse,
            string tokenEndpoint,
            string authorizationEndpoint,
            string registrationEndpoint,
            IList<string> redirectUris,
            TokenEndpointAuthMethodEnum tokenEndpointAuthMethod,
            ClientRegistrationModelsPublic.OBClientRegistration1 bankApiRequest,
            OAuth2RequestObjectClaimsOverrides? oAuth2RequestObjectClaimsOverrides,
            string externalApiId,
            string? externalApiSecret,
            string? registrationAccessToken,
            ClientRegistrationModelsPublic.OBClientRegistration1Response bankApiResponse,
            Guid bankId) : base(
            name,
            reference,
            id,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy)
        {
            SoftwareStatementProfileId = softwareStatementProfileId;
            SoftwareStatementAndCertificateProfileOverrideCase = softwareStatementAndCertificateProfileOverrideCase;
            RegistrationScope = registrationScope;
            ClientRegistrationApi = clientRegistrationApi;
            OpenIdConfigurationResponse = openIdConfigurationResponse;
            TokenEndpoint = tokenEndpoint;
            AuthorizationEndpoint = authorizationEndpoint;
            RegistrationEndpoint = registrationEndpoint;
            RedirectUris = redirectUris;
            TokenEndpointAuthMethod = tokenEndpointAuthMethod;
            BankApiRequest = bankApiRequest;
            OAuth2RequestObjectClaimsOverrides = oAuth2RequestObjectClaimsOverrides;
            ExternalApiId = externalApiId;
            ExternalApiSecret = externalApiSecret;
            RegistrationAccessToken = registrationAccessToken;
            BankApiResponse = bankApiResponse;
            BankId = bankId;
        }


        public string SoftwareStatementProfileId { get; }

        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; }

        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        public RegistrationScopeEnum RegistrationScope { get; }

        public ClientRegistrationApiVersion ClientRegistrationApi { get; }

        /// <summary>
        ///     Log of OpenID Configuration provided by well-known endpoint. Archived for debugging and should not be accessed by
        ///     Open Banking Connector.
        /// </summary>
        public OpenIdConfiguration OpenIdConfigurationResponse { get; }

        /// <summary>
        ///     Token endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string TokenEndpoint { get; }

        /// <summary>
        ///     Authorization endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string AuthorizationEndpoint { get; }

        /// <summary>
        ///     Registration endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string RegistrationEndpoint { get; }

        /// <summary>
        ///     Redirect URIs valid for this registration
        /// </summary>
        public IList<string> RedirectUris { get; }

        /// <summary>
        ///     Token endpoint authorisation method
        /// </summary>
        public TokenEndpointAuthMethodEnum TokenEndpointAuthMethod { get; }

        public ClientRegistrationModelsPublic.OBClientRegistration1 BankApiRequest { get; }

        public OAuth2RequestObjectClaimsOverridesRequest? OAuth2RequestObjectClaimsOverrides { get; }

        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public IList<DomesticPaymentConsent> DomesticPaymentConsentsNavigation { get; } =
            new List<DomesticPaymentConsent>();

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }

        /// <summary>
        ///     External API secret. Present to allow use of legacy token auth method "client_secret_basic" in sandboxes etc.
        /// </summary>
        public string? ExternalApiSecret { get; }

        /// <summary>
        ///     External API registration access token. Sometimes used to support registration adjustments etc.
        /// </summary>
        public string? RegistrationAccessToken { get; }

        public ClientRegistrationModelsPublic.OBClientRegistration1Response BankApiResponse { get; }

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public Guid BankId { get; }
    }

    internal partial class BankRegistration :
        ISupportsFluentLocalEntityGet<BankRegistrationResponse>
    {
        public BankRegistrationResponse PublicGetLocalResponse => new BankRegistrationResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            BankApiResponse,
            BankId);
    }
}
