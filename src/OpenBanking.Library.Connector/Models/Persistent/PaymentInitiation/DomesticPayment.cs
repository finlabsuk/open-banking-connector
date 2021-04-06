// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class DomesticPayment :
        EntityBase,
        ISupportsFluentPost<DomesticPaymentRequest, DomesticPaymentPostResponse>,
        ISupportsFluentGetLocal<DomesticPayment, IDomesticPaymentPublicQuery, DomesticPaymentGetLocalResponse>,
        //ISupportsFluentGet<DomesticPaymentResponse>,
        ISupportsFluentDeleteLocal<DomesticPayment>,
        IDomesticPaymentPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public DomesticPayment() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public DomesticPayment(
            Guid domesticPaymentConsentId,
            PaymentInitiationModelsPublic.OBWriteDomesticResponse5 obWriteDomesticResponse,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, createdBy, timeProvider)
        {
            DomesticPaymentConsentId = domesticPaymentConsentId;
            OBWriteDomesticResponse = obWriteDomesticResponse;
        }

        public Guid DomesticPaymentConsentId { get; set; }

        public string GetPath => "/domestic-payments";

        public DomesticPaymentPostResponse PublicPostResponse =>
            new DomesticPaymentPostResponse(OBWriteDomesticResponse);

        public PaymentInitiationModelsPublic.OBWriteDomesticResponse5 OBWriteDomesticResponse { get; set; } = null!;
        //public GetAsyncWrapperDelegate<DomesticPaymentResponse> GetAsyncWrapper => GetAsync;

        public DomesticPaymentGetLocalResponse PublicGetLocalResponse =>
            new DomesticPaymentGetLocalResponse(OBWriteDomesticResponse);

        public PostEntityAsyncWrapperDelegate<DomesticPaymentRequest, DomesticPaymentPostResponse>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        // public static async Task<(DomesticPaymentResponse response, IList<IFluentResponseInfoOrWarningMessage>
        //         nonErrorMessages)>
        //     GetAsync(
        //         Guid id,
        //         ISharedContext context)
        // {
        //     GetEntity<DomesticPayment, DomesticPaymentResponse> i =
        //         new GetEntity<DomesticPayment, DomesticPaymentResponse>(
        //             context.DbService.GetDbEntityMethodsClass<DomesticPayment>());
        //
        //     (DomesticPaymentResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) result =
        //         await i.GetAsync(id);
        //     return result;
        // }

        public static async Task<(DomesticPaymentPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostEntityAsync(
                ISharedContext context,
                DomesticPaymentRequest request,
                string? createdBy)
        {
            PostDomesticPayment i = new PostDomesticPayment(
                context.DbService.GetDbEntityMethodsClass<Bank>(),
                context.DbService.GetDbEntityMethodsClass<BankApiInformation>(),
                context.DbService
                    .GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.ApiVariantMapper,
                context.DbService
                    .GetDbEntityMethodsClass<BankRegistration>(),
                context.DbService
                    .GetDbEntityMethodsClass<DomesticPayment>(),
                context.TimeProvider,
                context.DbService.GetDbSaveChangesMethodClass(),
                context.SoftwareStatementProfileCachedRepo,
                context.Instrumentation);

            (DomesticPaymentPostResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) result =
                await i.PostAsync(request, createdBy);
            return result;
        }
    }
}
