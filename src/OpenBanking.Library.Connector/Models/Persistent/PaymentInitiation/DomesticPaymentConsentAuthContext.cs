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

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal class DomesticPaymentConsentAuthContext :
        EntityBase,
        ISupportsFluentPost<Public.PaymentInitiation.Request.DomesticPaymentConsentAuthContext,
            DomesticPaymentConsentAuthContextPostResponse>,
        ISupportsFluentGetLocal<DomesticPaymentConsentAuthContext, IDomesticPaymentConsentAuthContextPublicQuery,
            DomesticPaymentConsentAuthContextGetLocalResponse>,
        ISupportsFluentDeleteLocal<DomesticPaymentConsentAuthContext>,
        IDomesticPaymentConsentAuthContextPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public DomesticPaymentConsentAuthContext() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public DomesticPaymentConsentAuthContext(
            Guid domesticPaymentConsentId,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, createdBy, timeProvider)
        {
            DomesticPaymentConsentId = domesticPaymentConsentId;
        }

        public Guid DomesticPaymentConsentId { get; set; }

        /// <summary>
        ///     Token endpoint response. If null, indicates auth not successfully completed.
        /// </summary>
        public ReadWriteProperty<TokenEndpointResponse?> TokenEndpointResponse { get; set; } = null!;

        public DomesticPaymentConsentAuthContextGetLocalResponse PublicGetLocalResponse =>
            new DomesticPaymentConsentAuthContextGetLocalResponse();

        public PostEntityAsyncWrapperDelegate<Public.PaymentInitiation.Request.DomesticPaymentConsentAuthContext,
                DomesticPaymentConsentAuthContextPostResponse>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public DomesticPaymentConsentAuthContextPostResponse PublicPostResponse(string authUrl) =>
            new DomesticPaymentConsentAuthContextPostResponse(authUrl);


        public static async Task<(DomesticPaymentConsentAuthContextPostResponse response,
                IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostEntityAsync(
                ISharedContext context,
                Public.PaymentInitiation.Request.DomesticPaymentConsentAuthContext request,
                string? createdBy)
        {
            PostDomesticPaymentConsentAuthContext i = new PostDomesticPaymentConsentAuthContext(
                context.SoftwareStatementProfileCachedRepo,
                context.DbService.GetDbEntityMethodsClass<Bank>(),
                context.DbService.GetDbEntityMethodsClass<BankRegistration>(),
                context.DbService.GetDbEntityMethodsClass<BankApiInformation>(),
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                context.DbService.GetDbEntityMethodsClass<DomesticPaymentConsentAuthContext>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.Instrumentation,
                context.TimeProvider);
            (DomesticPaymentConsentAuthContextPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages) result =
                    await i.PostAsync(request, createdBy);
            return result;
        }
    }
}
