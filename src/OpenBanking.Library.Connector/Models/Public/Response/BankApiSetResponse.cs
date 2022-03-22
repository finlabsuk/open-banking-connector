// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBankApiSetPublicQuery : IBaseQuery
    {
        Guid BankId { get; }
        AccountAndTransactionApi? AccountAndTransactionApi { get; }
        PaymentInitiationApi? PaymentInitiationApi { get; }
        public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class BankApiSetResponse : BaseResponse, IBankApiSetPublicQuery
    {
        internal BankApiSetResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            AccountAndTransactionApi? accountAndTransactionApi,
            PaymentInitiationApi? paymentInitiationApi,
            VariableRecurringPaymentsApi? variableRecurringPaymentsApi,
            Guid bankId) : base(id, name, created, createdBy)
        {
            BankId = bankId;
            AccountAndTransactionApi = accountAndTransactionApi;
            PaymentInitiationApi = paymentInitiationApi;
            VariableRecurringPaymentsApi = variableRecurringPaymentsApi;
        }

        public Guid BankId { get; }
        public AccountAndTransactionApi? AccountAndTransactionApi { get; }
        public PaymentInitiationApi? PaymentInitiationApi { get; }
        public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi { get; }
    }
}
