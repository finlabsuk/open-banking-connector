// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
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
        ITransactionsContext<TransactionsResponse> Transactions { get; }
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
                new AccountGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(_sharedContext.ApiClient),
                    new AuthContextAccessTokenGet(
                        _sharedContext.SoftwareStatementProfileCachedRepo,
                        _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                        _sharedContext.TimeProvider,
                        new GrantPost(_sharedContext.ApiClient))));

        public IReadOnlyExternalEntityContext<BalancesResponse> Balances =>
            new ReadOnlyExternalEntityContextInternal<BalancesResponse>(
                new BalanceGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(_sharedContext.ApiClient),
                    new AuthContextAccessTokenGet(
                        _sharedContext.SoftwareStatementProfileCachedRepo,
                        _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                        _sharedContext.TimeProvider,
                        new GrantPost(_sharedContext.ApiClient))));

        public IReadOnlyExternalEntityContext<PartiesResponse> Parties =>
            new ReadOnlyExternalEntityContextInternal<PartiesResponse>(
                new PartyGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(_sharedContext.ApiClient),
                    new AuthContextAccessTokenGet(
                        _sharedContext.SoftwareStatementProfileCachedRepo,
                        _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                        _sharedContext.TimeProvider,
                        new GrantPost(_sharedContext.ApiClient))));

        public IReadOnlyExternalEntityContext<Parties2Response> Parties2 =>
            new ReadOnlyExternalEntityContextInternal<Parties2Response>(
                new Party2Get(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(_sharedContext.ApiClient),
                    new AuthContextAccessTokenGet(
                        _sharedContext.SoftwareStatementProfileCachedRepo,
                        _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                        _sharedContext.TimeProvider,
                        new GrantPost(_sharedContext.ApiClient))));

        public ITransactionsContext<TransactionsResponse> Transactions =>
            new TransactionsContextInternal<TransactionsResponse>(
                _sharedContext,
                new TransactionGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<AccountAccessConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(_sharedContext.ApiClient),
                    new AuthContextAccessTokenGet(
                        _sharedContext.SoftwareStatementProfileCachedRepo,
                        _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                        _sharedContext.TimeProvider,
                        new GrantPost(_sharedContext.ApiClient))));
    }
}
