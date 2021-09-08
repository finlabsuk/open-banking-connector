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
using Microsoft.EntityFrameworkCore;
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
    [Index(nameof(Name), IsUnique = true)]
    internal partial class BankRegistration :
        EntityBase,
        ISupportsFluentDeleteLocal<BankRegistration>,
        IBankRegistrationPublicQuery
    {
        public string SoftwareStatementProfileId { get; set; } = null!;

        /// <summary>
        ///     Functional APIs used for bank registration.
        /// </summary>
        public RegistrationScope RegistrationScope { get; set; }

        public ClientRegistrationApiVersion ClientRegistrationApi { get; set; }

        public OpenIdConfiguration OpenIdConfiguration { get; set; } = null!;

        public ClientRegistrationModelsPublic.OBClientRegistration1 BankApiRequest { get; set; } = null!;

        public OAuth2RequestObjectClaimsOverridesRequest? OAuth2RequestObjectClaimsOverrides { get; set; }

        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public List<DomesticPaymentConsent> DomesticPaymentConsentsNavigation { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        public ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response> BankApiResponse
        {
            get;
            set;
        } = null!;

        /// <summary>
        ///     Bank this registration is with.
        /// </summary>
        public Guid BankId { get; set; }
    }

    internal partial class BankRegistration :
        ISupportsFluentEntityPost<BankRegistrationRequest, BankRegistrationResponse,
            ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response>
    {
        public BankRegistrationResponse PublicGetResponse => new BankRegistrationResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            BankApiResponse,
            BankId);

        public void Initialise(
            BankRegistrationRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            ClientRegistrationApi = request.ClientRegistrationApi;
            SoftwareStatementProfileId = request.SoftwareStatementProfileId;
            OAuth2RequestObjectClaimsOverrides = request.OAuth2RequestObjectClaimsOverrides;
            BankId = request.BankId;
        }

        public BankRegistrationResponse PublicPostResponse => PublicGetResponse;

        public void UpdateBeforeApiPost(ClientRegistrationModelsPublic.OBClientRegistration1 apiRequest)
        {
            BankApiRequest = apiRequest;
        }

        public void UpdateAfterApiPost(
            ClientRegistrationModelsPublic.OBClientRegistration1Response apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
            ExternalApiId = BankApiResponse.Data.ClientId;
        }

        public void UpdateOpenIdGet(
            RegistrationScope registrationScope,
            OpenIdConfiguration openIdConfiguration)
        {
            RegistrationScope = registrationScope;
            OpenIdConfiguration = openIdConfiguration;
        }

        public IApiPostRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
            ClientRegistrationModelsPublic.OBClientRegistration1Response> ApiPostRequests(
            ClientRegistrationApiVersion clientRegistrationApiVersion,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            clientRegistrationApiVersion switch
            {
                ClientRegistrationApiVersion.Version3p1 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsV3p1.OBClientRegistration1,
                        ClientRegistrationModelsV3p1.OBClientRegistration1>(
                        new JwtRequestProcessor<ClientRegistrationModelsV3p1.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient),
                        new JwtRequestProcessor<ClientRegistrationModelsV3p1.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient)),
                ClientRegistrationApiVersion.Version3p2 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsV3p2.OBClientRegistration1,
                        ClientRegistrationModelsV3p2.OBClientRegistration1>(
                        new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient),
                        new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient)),
                ClientRegistrationApiVersion.Version3p3 =>
                    new ApiRequests<ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response,
                        ClientRegistrationModelsPublic.OBClientRegistration1,
                        ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                        new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient),
                        new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                            processedSoftwareStatementProfile,
                            instrumentationClient)),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(clientRegistrationApiVersion),
                    clientRegistrationApiVersion,
                    null)
            };
    }

    internal partial class BankRegistration :
        ISupportsFluentEntityGet<BankRegistrationResponse,
            ClientRegistrationModelsPublic.OBClientRegistration1Response>
    {
        public void UpdateAfterApiGet(
            ClientRegistrationModelsPublic.OBClientRegistration1Response apiResponse,
            string? modifiedBy,
            ITimeProvider timeProvider)
        {
            BankApiResponse =
                new ReadWriteProperty<ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                    apiResponse,
                    timeProvider,
                    modifiedBy);
        }
    }
}
