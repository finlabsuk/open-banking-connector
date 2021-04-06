// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class DomesticPaymentConsent :
        EntityBase,
        ISupportsFluentPost<DomesticPaymentConsentRequest, DomesticPaymentConsentPostResponse>,
        ISupportsFluentGetLocal<DomesticPaymentConsent, IDomesticPaymentConsentPublicQuery,
            DomesticPaymentConsentGetLocalResponse>,
        ISupportsFluentGet<DomesticPaymentConsentGetResponse>,
        ISupportsFluentDeleteLocal<DomesticPaymentConsent>,
        IDomesticPaymentConsentPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public DomesticPaymentConsent() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public DomesticPaymentConsent(
            string state,
            Guid bankRegistrationId,
            Guid bankApiInformationId,
            PaymentInitiationModelsPublic.OBWriteDomesticConsent4 obWriteDomesticConsent,
            PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 obWriteDomesticConsentResponse,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, createdBy, timeProvider)
        {
            State = new ReadWriteProperty<string>(state, timeProvider, createdBy);
            TokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                new TokenEndpointResponse(),
                timeProvider,
                createdBy);
            BankRegistrationId = bankRegistrationId;
            BankApiInformationId = bankApiInformationId;
            OBWriteDomesticConsent = obWriteDomesticConsent;
            OBWriteDomesticConsentResponse = obWriteDomesticConsentResponse;
        }

        public ReadWriteProperty<string> State { get; set; } = null!;

        public PaymentInitiationModelsPublic.OBWriteDomesticConsent4 OBWriteDomesticConsent { get; set; } = null!;

        public ReadWriteProperty<TokenEndpointResponse?> TokenEndpointResponse { get; set; } = null!;

        public string OBId => OBWriteDomesticConsentResponse.Data.ConsentId;

        public Guid BankRegistrationId { get; set; }

        public Guid BankApiInformationId { get; set; }

        public PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 OBWriteDomesticConsentResponse
        {
            get;
            set;
        } =
            null!;

        public DomesticPaymentConsentGetResponse PublicGetResponse =>
            new DomesticPaymentConsentGetResponse(
                Id,
                OBWriteDomesticConsentResponse,
                BankRegistrationId,
                BankApiInformationId);
        public GetAsyncWrapperDelegate<DomesticPaymentConsentGetResponse> GetAsyncWrapper => GetAsync;

        public string ExternalApiPath => "/domestic-payment-consents";

        public DomesticPaymentConsentGetLocalResponse PublicGetLocalResponse =>
            new DomesticPaymentConsentGetLocalResponse(
                Id,
                OBWriteDomesticConsentResponse,
                BankRegistrationId,
                BankApiInformationId);

        public PostEntityAsyncWrapperDelegate<DomesticPaymentConsentRequest, DomesticPaymentConsentPostResponse>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public DomesticPaymentConsentPostResponse PublicPostResponse(string authUrl) =>
            new DomesticPaymentConsentPostResponse(
                authUrl,
                Id,
                OBWriteDomesticConsentResponse,
                BankRegistrationId,
                BankApiInformationId);

        public static async Task<(DomesticPaymentConsentGetResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            GetAsync(
                Guid id,
                ISharedContext context)
        {
            GetDomesticPaymentConsent i =
                new GetDomesticPaymentConsent(
                    context.DbService.GetDbEntityMethodsClass<BankApiInformation>(),
                    context.DbService.GetDbEntityMethodsClass<BankRegistration>(),
                    context.DbService.GetDbEntityMethodsClass<Bank>(),
                    context.DbService.GetDbSaveChangesMethodClass(),
                    context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                    context.Instrumentation,
                    context.ApiVariantMapper,
                    context.SoftwareStatementProfileCachedRepo,
                    context.TimeProvider);

            (DomesticPaymentConsentGetResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)
                result =
                    await i.GetAsync(id);
            return result;
        }

        public static async Task<(DomesticPaymentConsentPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostEntityAsync(
                ISharedContext context,
                DomesticPaymentConsentRequest request,
                string? createdBy)
        {
            PostDomesticPaymentConsent i = new PostDomesticPaymentConsent(
                context.DbService.GetDbEntityMethodsClass<BankApiInformation>(),
                context.DbService.GetDbEntityMethodsClass<BankRegistration>(),
                context.DbService.GetDbEntityMethodsClass<Bank>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.ApiVariantMapper,
                context.TimeProvider,
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation);

            (DomesticPaymentConsentPostResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)
                result =
                    await i.PostAsync(request, createdBy);
            return result;
        }
    }
}
