// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using BankRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.Bank;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for Bank.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    internal class Bank :
        EntityBase,
        ISupportsFluentPost<BankRequest, BankPostResponse>,
        ISupportsFluentGetLocal<Bank, IBankPublicQuery, BankGetLocalResponse>,
        ISupportsFluentDeleteLocal<Bank>,
        IBankPublicQuery
    {
        /// <summary>
        ///     Constructor intended for use by EF Core and to access static methods in generic context only.
        ///     Should not be used to create entity instances to add to DB.
        /// </summary>
        public Bank() { }

        /// <summary>
        ///     Constructor for creating entity instances to add to DB.
        /// </summary>
        public Bank(
            string issuerUrl,
            string financialId,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, createdBy, timeProvider)
        {
            IssuerUrl = issuerUrl;
            FinancialId = financialId;
        }

        public BankPostResponse PublicPostResponse => new BankPostResponse(
            IssuerUrl,
            FinancialId,
            Name,
            Id);

        public string IssuerUrl { get; } = null!;

        public string FinancialId { get; } = null!;

        public BankGetLocalResponse PublicGetLocalResponse => new BankGetLocalResponse(
            IssuerUrl,
            FinancialId,
            Name,
            Id);

        public PostEntityAsyncWrapperDelegate<BankRequest, BankPostResponse> PostEntityAsyncWrapper =>
            PostEntityAsync;

        public static async Task<(BankPostResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)> PostEntityAsync(
            ISharedContext context,
            BankRequest requestBank,
            string? createdBy)
        {
            PostBank i = new PostBank(
                context.DbService.GetDbEntityMethodsClass<Bank>(),
                context.DbService.GetDbSaveChangesMethodClass(),
                context.TimeProvider);

            (BankPostResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) result =
                await i.PostAsync(requestBank, createdBy);

            return result;
        }
    }
}
