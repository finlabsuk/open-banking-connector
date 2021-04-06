// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using BankApiInformationRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankApiInformation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for BankProfile.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    internal class BankApiInformation :
        EntityBase,
        ISupportsFluentPost<BankApiInformationRequest, BankApiInformationPostResponse>,
        ISupportsFluentGetLocal<BankApiInformation, IBankApiInformationPublicQuery, BankApiInformationGetLocalResponse>,
        ISupportsFluentDeleteLocal<BankApiInformation>,
        IBankApiInformationPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access customisation points defined by IEntityWithPublicInterface in
        ///     generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public BankApiInformation() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public BankApiInformation(
            PaymentInitiationApi? paymentInitiationApi,
            Guid bankId,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, createdBy, timeProvider)
        {
            PaymentInitiationApi = paymentInitiationApi;
            BankId = bankId;
        }

        public BankApiInformationPostResponse PublicPostResponse => new BankApiInformationPostResponse(
            PaymentInitiationApi,
            Id,
            BankId);

        public Guid BankId { get; set; }

        public PaymentInitiationApi? PaymentInitiationApi { get; set; }

        public BankApiInformationGetLocalResponse PublicGetLocalResponse => new BankApiInformationGetLocalResponse(
            PaymentInitiationApi,
            Id,
            BankId);

        public PostEntityAsyncWrapperDelegate<BankApiInformationRequest, BankApiInformationPostResponse>
            PostEntityAsyncWrapper =>
            PostEntityAsync;

        public static async Task<(BankApiInformationPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)>
            PostEntityAsync(
                ISharedContext context,
                BankApiInformationRequest request,
                string? createdBy)
        {
            PostBankApiInformation i = new PostBankApiInformation(
                context.DbService.GetDbEntityMethodsClass<BankApiInformation>(),
                context.DbService.GetDbEntityMethodsClass<Bank>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider);

            (BankApiInformationPostResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) result =
                await i.PostAsync(request, createdBy);
            return result;
        }
    }
}
