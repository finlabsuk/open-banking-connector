// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using BankApiSetRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.BankApiSet;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type for Bank API Set
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    internal partial class BankApiSet :
        EntityBase,
        ISupportsFluentDeleteLocal<BankApiSet>,
        IBankApiSetPublicQuery
    {
        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public List<DomesticPaymentConsent> DomesticPaymentConsentsNavigation { get; set; } = null!;

        public PaymentInitiationApi? PaymentInitiationApi { get; set; }

        public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi { get; set; }

        public Guid BankId { get; set; }
    }

    internal partial class BankApiSet :
        ISupportsFluentLocalEntityPost<BankApiSetRequest, BankApiSetResponse>
    {
        public BankApiSetResponse PublicGetResponse => new BankApiSetResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            PaymentInitiationApi,
            VariableRecurringPaymentsApi,
            BankId);

        public void Initialise(
            BankApiSetRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            base.Initialise(Guid.NewGuid(), request.Name, createdBy, timeProvider);
            PaymentInitiationApi = request.PaymentInitiationApi;
            VariableRecurringPaymentsApi = request.VariableRecurringPaymentsApi;
            BankId = request.BankId;
        }

        public BankApiSetResponse PublicPostResponse => PublicGetResponse;
    }

    internal partial class BankApiSet :
        ISupportsFluentLocalEntityGet<BankApiSetResponse> { }
}
