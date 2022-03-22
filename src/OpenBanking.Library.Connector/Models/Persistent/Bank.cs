// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using BankRequest = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.Bank;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for Bank.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class Bank :
        EntityBase,
        IBankPublicQuery
    {
        /// <summary>
        ///     Constructor (to be removed) that doesn't initialise anything
        /// </summary>
        public Bank() { }

        /// <summary>
        ///     New constructor that initialises get-only properties
        /// </summary>
        /// <param name="issuerUrl"></param>
        /// <param name="financialId"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="createdBy"></param>
        /// <param name="timeProvider"></param>
        private Bank(
            string issuerUrl,
            string financialId,
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider) : base(
            id,
            name,
            createdBy,
            timeProvider)
        {
            IssuerUrl = issuerUrl;
            FinancialId = financialId;
        }

        public IList<BankRegistration> BankRegistrationsNavigation { get; set; } = null!;

        public IList<BankApiSet> BankApiSetsNavigation { get; set; } = null!;

        public string IssuerUrl { get; set; } = null!;

        public string FinancialId { get; set; } = null!;

        public BankResponse PublicGetLocalResponse => new BankResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            IssuerUrl,
            FinancialId);
    }

    internal partial class Bank :
        ISupportsFluentLocalEntityPost<BankRequest, BankResponse, Bank>
    {
        public BankResponse PublicPostLocalResponse => PublicGetLocalResponse;

        public Bank Create(
            BankRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var output = new Bank(
                request.IssuerUrl,
                request.FinancialId,
                Guid.NewGuid(),
                request.Name,
                createdBy,
                timeProvider);

            return output;
        }
    }

    internal partial class Bank :
        ISupportsFluentLocalEntityGet<BankResponse> { }
}
