// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
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
    internal partial class Bank :
        EntityBase,
        ISupportsFluentDeleteLocal<Bank>,
        IBankPublicQuery
    {
        public List<BankRegistration> BankRegistrationsNavigation { get; set; } = null!;

        public List<BankApiSet> BankApiSetsNavigation { get; set; } = null!;

        public string IssuerUrl { get; set; } = null!;

        public string FinancialId { get; set; } = null!;

        public BankResponse PublicGetResponse => new BankResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            IssuerUrl,
            FinancialId);
    }

    internal partial class Bank :
        ISupportsFluentLocalEntityPost<BankRequest, BankResponse>
    {
        public void Initialise(
            BankRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            IssuerUrl = request.IssuerUrl;
            FinancialId = request.FinancialId;
        }

        public BankResponse PublicPostResponse => PublicGetResponse;
    }

    internal partial class Bank :
        ISupportsFluentLocalEntityGet<BankResponse> { }
}
