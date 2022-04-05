// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction
{
    public interface IAccountAndTransactionContext
    {
        /// <summary>
        ///     API for AccountAccessConsent objects.
        ///     A AccountAccessConsent is an Open Banking object and corresponds to an "intended" account access.
        /// </summary>
        IAccountAccessConsentsContext AccountAccessConsents { get; }

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
        ///     API for Party objects.
        /// </summary>
        IReadOnlyExternalEntityContext<Parties2Response> Parties2 { get; }

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

        public IAccountAccessConsentsContext AccountAccessConsents =>
            new AccountAccessConsentsConsentContext(_sharedContext);

        public IReadOnlyExternalEntityContext<AccountsResponse> Accounts =>
            new ReadOnlyExternalEntityContextInternal<AccountsResponse>(
                _sharedContext,
                new AccountGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper));

        public IReadOnlyExternalEntityContext<BalancesResponse> Balances =>
            new ReadOnlyExternalEntityContextInternal<BalancesResponse>(
                _sharedContext,
                new BalanceGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper));

        public IReadOnlyExternalEntityContext<PartiesResponse> Parties =>
            new ReadOnlyExternalEntityContextInternal<PartiesResponse>(
                _sharedContext,
                new PartyGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper));

        public IReadOnlyExternalEntityContext<Parties2Response> Parties2 =>
            new ReadOnlyExternalEntityContextInternal<Parties2Response>(
                _sharedContext,
                new Party2Get(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper));

        public IReadOnlyExternalEntityContext<TransactionsResponse> Transactions =>
            new ReadOnlyExternalEntityContextInternal<TransactionsResponse>(
                _sharedContext,
                new TransactionGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper));
    }
}
