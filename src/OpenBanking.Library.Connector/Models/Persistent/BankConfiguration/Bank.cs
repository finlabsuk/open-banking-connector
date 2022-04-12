// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration
{
    /// <summary>
    ///     Persisted type for Bank.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class Bank :
        BaseEntity,
        IBankPublicQuery
    {
        public Bank(
            string? name,
            string? reference,
            Guid id,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string issuerUrl,
            string financialId) : base(
            name,
            reference,
            id,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy)
        {
            IssuerUrl = issuerUrl;
            FinancialId = financialId;
        }


        public IList<BankRegistration> BankRegistrationsNavigation { get; } = new List<BankRegistration>();

        public IList<BankApiSet> BankApiSetsNavigation { get; } = new List<BankApiSet>();

        public string IssuerUrl { get; }

        public string FinancialId { get; }
    }

    internal partial class Bank :
        ISupportsFluentLocalEntityGet<BankResponse>
    {
        public BankResponse PublicGetLocalResponse => new BankResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            IssuerUrl,
            FinancialId);
    }
}
