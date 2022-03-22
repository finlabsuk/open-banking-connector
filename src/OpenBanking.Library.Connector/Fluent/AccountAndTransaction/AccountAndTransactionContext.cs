// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction
{
    public interface IAccountAndTransactionContext
    {
        /// <summary>
        ///     API for Account objects.
        /// </summary>
        IReadOnlyExternalEntityContext<AccountsResponse> Accounts { get; }

        /// <summary>
        ///     API for Balance objects.
        /// </summary>
        IReadOnlyExternalEntityContext<BalancesResponse> Balances { get; }

        /// <summary>
        ///     API for Party objects.
        /// </summary>
        IReadOnlyExternalEntityContext<PartiesResponse> Parties { get; }

        /// <summary>
        ///     API for Transaction objects.
        /// </summary>
        IReadOnlyExternalEntityContext<TransactionsResponse> Transactions { get; }
    }

    internal class AccountAndTransactionContext : IAccountAndTransactionContext
    {
        private readonly ISharedContext _sharedContext;

        public AccountAndTransactionContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IReadOnlyExternalEntityContext<AccountsResponse> Accounts { get; } = null!;
        public IReadOnlyExternalEntityContext<BalancesResponse> Balances { get; } = null!;
        public IReadOnlyExternalEntityContext<PartiesResponse> Parties { get; } = null!;
        public IReadOnlyExternalEntityContext<TransactionsResponse> Transactions { get; } = null!;
    }
}
