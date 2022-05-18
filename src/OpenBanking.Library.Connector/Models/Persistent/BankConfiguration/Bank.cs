// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
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
            string? reference,
            Guid id,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string issuerUrl,
            string financialId) : base(
            id,
            reference,
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

        public IList<AccountAndTransactionApiEntity> AccountAndTransactionApisNavigation { get; } =
            new List<AccountAndTransactionApiEntity>();

        public IList<PaymentInitiationApiEntity> PaymentInitiationApisNavigation { get; } =
            new List<PaymentInitiationApiEntity>();

        public IList<VariableRecurringPaymentsApiEntity> VariableRecurringPaymentsApisNavigation { get; } =
            new List<VariableRecurringPaymentsApiEntity>();

        public string IssuerUrl { get; }

        public string FinancialId { get; }
    }

    internal partial class Bank :
        ISupportsFluentLocalEntityGet<BankResponse>
    {
        public BankResponse PublicGetLocalResponse => new(
            Id,
            Created,
            CreatedBy,
            Reference,
            IssuerUrl,
            FinancialId);
    }
}
