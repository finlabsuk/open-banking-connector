// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using BankApiSetRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankApiSet;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for Bank API Set
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class BankApiSet :
        EntityBase,
        IBankApiSetPublicQuery
    {
        public BankApiSet() { }

        private BankApiSet(
            Guid id,
            string? name,
            BankApiSetRequest request,
            string? createdBy,
            ITimeProvider timeProvider) : base(
            id,
            name,
            createdBy,
            timeProvider)
        {
            AccountAndTransactionApi = request.AccountAndTransactionApi;
            PaymentInitiationApi = request.PaymentInitiationApi;
            VariableRecurringPaymentsApi = request.VariableRecurringPaymentsApi;
            BankId = request.BankId;
        }

        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public IList<DomesticPaymentConsent> DomesticPaymentConsentsNavigation { get; set; } = null!;

        public AccountAndTransactionApi? AccountAndTransactionApi { get; set; }

        public PaymentInitiationApi? PaymentInitiationApi { get; set; }

        public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi { get; set; }

        public Guid BankId { get; set; }
    }

    internal partial class BankApiSet :
        ISupportsFluentLocalEntityPost<BankApiSetRequest, BankApiSetResponse, BankApiSet>
    {
        public BankApiSetResponse PublicPostLocalResponse => PublicGetLocalResponse;

        public BankApiSet Create(
            BankApiSetRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var output = new BankApiSet(
                Guid.NewGuid(),
                request.Name,
                request,
                createdBy,
                timeProvider);

            return output;
        }
    }

    internal partial class BankApiSet :
        ISupportsFluentLocalEntityGet<BankApiSetResponse>
    {
        public BankApiSetResponse PublicGetLocalResponse => new BankApiSetResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            AccountAndTransactionApi,
            PaymentInitiationApi,
            VariableRecurringPaymentsApi,
            BankId);
    }
}
