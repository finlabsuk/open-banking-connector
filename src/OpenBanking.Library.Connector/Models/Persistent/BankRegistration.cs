// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using BankRegistrationRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankRegistration;
using OAuth2RequestObjectClaimsOverridesRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.OAuth2RequestObjectClaimsOverrides;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using ClientRegistrationModelsV3p2 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p2.Models;
using ClientRegistrationModelsV3p1 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class BankRegistration :
        EntityBase,
        IBankRegistrationPublicQuery
    {
        public BankRegistration() { }

        private BankRegistration(
            Guid id,
            string? name,
            BankRegistrationRequest request,
            ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest,
            ClientRegistrationModelsPublic.OBClientRegistration1Response apiResponse,
            RegistrationScope registrationScope,
            OpenIdConfiguration openIdConfigurationResponse,
            string tokenEndpoint,
            string authorizationEndpoint,
            string registrationEndpoint,
            TokenEndpointAuthMethodEnum tokenEndpointAuthMethod,
            string? createdBy,
            ITimeProvider timeProvider) : base(
            id,
            name,
            createdBy,
            timeProvider)
        {
            ClientRegistrationApi = request.ClientRegistrationApi;
            SoftwareStatementProfileId = request.SoftwareStatementProfileId;
            SoftwareStatementAndCertificateProfileOverrideCase =
                request.SoftwareStatementAndCertificateProfileOverrideCase;
            OAuth2RequestObjectClaimsOverrides = request.OAuth2RequestObjectClaimsOverrides;
            BankId = request.BankId;

            BankApiRequest = apiRequest;

            RegistrationScope = registrationScope;
            OpenIdConfigurationResponse = openIdConfigurationResponse;
            TokenEndpoint = tokenEndpoint;
            AuthorizationEndpoint = authorizationEndpoint;
            RegistrationEndpoint = registrationEndpoint;
            TokenEndpointAuthMethod = tokenEndpointAuthMethod;

            BankApiResponse =
                new ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                    apiResponse,
                    timeProvider,
                    createdBy);
            ExternalApiId = BankApiResponse.Value.ClientId;
            ExternalApiSecret = BankApiResponse.Value.ClientSecret;
            RegistrationAccessToken = BankApiResponse.Value.RegistrationAccessToken;
            RedirectUris = BankApiResponse.Value.RedirectUris;
        }

        public string SoftwareStatementProfileId { get; set; } = null!;

        public string? SoftwareStatementAndCertificateProfileOverrideCase { get; set; }

        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        public RegistrationScope RegistrationScope { get; set; }

        public ClientRegistrationApiVersion ClientRegistrationApi { get; set; }

        /// <summary>
        ///     Log of OpenID Configuration provided by well-known endpoint. Archived for debugging and should not be accessed by
        ///     Open Banking Connector.
        /// </summary>
        public OpenIdConfiguration OpenIdConfigurationResponse { get; set; } = null!;

        /// <summary>
        ///     Token endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string TokenEndpoint { get; set; } = null!;

        /// <summary>
        ///     Authorization endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string AuthorizationEndpoint { get; set; } = null!;

        /// <summary>
        ///     Registration endpoint (normally supplied from OpenID Configuration)
        /// </summary>
        public string RegistrationEndpoint { get; set; } = null!;

        /// <summary>
        ///     Redirect URIs valid for this registration
        /// </summary>
        public IList<string> RedirectUris { get; set; } = new List<string>();

        /// <summary>
        ///     Token endpoint authorisation method
        /// </summary>
        public TokenEndpointAuthMethodEnum TokenEndpointAuthMethod { get; set; }

        public ClientRegistrationModelsPublic.OBClientRegistration1 BankApiRequest { get; set; } = null!;

        public OAuth2RequestObjectClaimsOverridesRequest? OAuth2RequestObjectClaimsOverrides { get; set; }

        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public IList<DomesticPaymentConsent> DomesticPaymentConsentsNavigation { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        /// <summary>
        ///     External API secret. Present to allow use of legacy token auth method "client_secret_basic" in sandboxes etc.
        /// </summary>
        public string? ExternalApiSecret { get; set; }

        /// <summary>
        ///     External API registration access token. Sometimes used to support registration adjustments etc.
        /// </summary>
        public string? RegistrationAccessToken { get; set; }

        public ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response> BankApiResponse
        {
            get;
            set;
        } = null!;

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public Guid BankId { get; set; }

        public BankRegistration Create(
            BankRegistrationRequest request,
            ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest,
            ClientRegistrationModelsPublic.OBClientRegistration1Response apiResponse,
            RegistrationScope registrationScope,
            OpenIdConfiguration openIdConfigurationResponse,
            string tokenEndpoint,
            string authorizationEndpoint,
            string registrationEndpoint,
            TokenEndpointAuthMethodEnum tokenEndpointAuthMethod,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var output = new BankRegistration(
                Guid.NewGuid(),
                request.Name,
                request,
                apiRequest,
                apiResponse,
                registrationScope,
                openIdConfigurationResponse,
                tokenEndpoint,
                authorizationEndpoint,
                registrationEndpoint,
                tokenEndpointAuthMethod,
                createdBy,
                timeProvider);

            return output;
        }

        public IApiPostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response> ApiPostRequests(
            ClientRegistrationApiVersion clientRegistrationApiVersion,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient,
            bool useApplicationJoseNotApplicationJwtContentTypeHeader) =>
            clientRegistrationApiVersion switch
            {
                ClientRegistrationApiVersion.Version3p1 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsV3p1.OBClientRegistration1,
                        ClientRegistrationModelsV3p1.OBClientRegistration1>(
                        new JwtRequestProcessor<ClientRegistrationModelsV3p1.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader),
                        new JwtRequestProcessor<ClientRegistrationModelsV3p1.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader)),
                ClientRegistrationApiVersion.Version3p2 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsV3p2.OBClientRegistration1,
                        ClientRegistrationModelsV3p2.OBClientRegistration1>(
                        new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader),
                        new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader)),
                ClientRegistrationApiVersion.Version3p3 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                        new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader),
                        new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient,
                            useApplicationJoseNotApplicationJwtContentTypeHeader)),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(clientRegistrationApiVersion),
                    clientRegistrationApiVersion,
                    null)
            };
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
