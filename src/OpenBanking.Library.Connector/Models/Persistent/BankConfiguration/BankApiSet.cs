// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration
{
    /// <summary>
    ///     Persisted type for Bank API Set
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class BankApiSet :
        BaseEntity,
        IBankApiSetPublicQuery
    {
        public BankApiSet(string? name, string? reference, Guid id, bool isDeleted, DateTimeOffset isDeletedModified, string? isDeletedModifiedBy, DateTimeOffset created, string? createdBy, VariableRecurringPaymentsApiVersionEnum? variableRecurringPaymentsApiVersion, string variableRecurringPaymentsApiBaseUrl, PaymentInitiationApiVersionEnum? paymentInitiationApiVersion, string paymentInitiationApiBaseUrl, Guid bankId) : base(name, reference, id, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
        {
            VariableRecurringPaymentsApiVersion = variableRecurringPaymentsApiVersion;
            VariableRecurringPaymentsApiBaseUrl = variableRecurringPaymentsApiBaseUrl;
            PaymentInitiationApiVersion = paymentInitiationApiVersion;
            PaymentInitiationApiBaseUrl = paymentInitiationApiBaseUrl;
            BankId = bankId;
        }

        [ForeignKey("BankId")]
        public Bank BankNavigation { get; set; } = null!;

        public IList<DomesticPaymentConsent> DomesticPaymentConsentsNavigation { get; } =
            new List<DomesticPaymentConsent>();

        public IList<DomesticVrpConsent> DomesticVrpConsentsNavigation { get; } =
            new List<DomesticVrpConsent>();

        public VariableRecurringPaymentsApiVersionEnum? VariableRecurringPaymentsApiVersion { get; }

        public string VariableRecurringPaymentsApiBaseUrl { get; }

        public PaymentInitiationApiVersionEnum? PaymentInitiationApiVersion { get; }

        public string PaymentInitiationApiBaseUrl { get; }

        public PaymentInitiationApi? PaymentInitiationApi =>
            PaymentInitiationApiVersion switch
            {
                null => null,
                { } apiVersion =>
                    new PaymentInitiationApi
                    {
                        PaymentInitiationApiVersion = apiVersion,
                        BaseUrl = PaymentInitiationApiBaseUrl
                    }
            };

        public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi =>
            VariableRecurringPaymentsApiVersion switch
            {
                null => null,
                { } apiVersion =>
                    new VariableRecurringPaymentsApi
                    {
                        VariableRecurringPaymentsApiVersion = apiVersion,
                        BaseUrl = VariableRecurringPaymentsApiBaseUrl
                    }
            };

        public Guid BankId { get; }
    }

    internal partial class BankApiSet :
        ISupportsFluentLocalEntityGet<BankApiSetResponse>
    {
        public BankApiSetResponse PublicGetLocalResponse => new BankApiSetResponse(
            Id,
            Name,
            Created,
            CreatedBy,
            BankId,
            PaymentInitiationApi,
            VariableRecurringPaymentsApi);
    }
}
