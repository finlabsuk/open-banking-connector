// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction;

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

    /// <summary>
    ///     API for Direct Debit objects.
    /// </summary>
    IReadOnlyExternalEntityContext<DirectDebitsResponse> DirectDebits { get; }

    /// <summary>
    ///     API for Standing Order objects.
    /// </summary>
    IReadOnlyExternalEntityContext<StandingOrdersResponse> StandingOrders { get; }

    /// <summary>
    ///     API for Monzo pot objects.
    /// </summary>
    IReadOnlyExternalEntityContext<MonzoPotsResponse> MonzoPots { get; }
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
        new ReadOnlyExternalEntityContext<AccountsResponse>(
            new AccountGet(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));

    public IReadOnlyExternalEntityContext<BalancesResponse> Balances =>
        new ReadOnlyExternalEntityContext<BalancesResponse>(
            new BalanceGet(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));

    public IReadOnlyExternalEntityContext<PartiesResponse> Parties =>
        new ReadOnlyExternalEntityContext<PartiesResponse>(
            new PartyGet(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));

    public IReadOnlyExternalEntityContext<Parties2Response> Parties2 =>
        new ReadOnlyExternalEntityContext<Parties2Response>(
            new Party2Get(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));

    public ITransactionsContext<TransactionsResponse> Transactions =>
        new TransactionsContext<TransactionsResponse>(
            _sharedContext,
            new TransactionGet(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));

    public IReadOnlyExternalEntityContext<DirectDebitsResponse> DirectDebits =>
        new ReadOnlyExternalEntityContext<DirectDebitsResponse>(
            new DirectDebitGet(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));

    public IReadOnlyExternalEntityContext<StandingOrdersResponse> StandingOrders =>
        new ReadOnlyExternalEntityContext<StandingOrdersResponse>(
            new StandingOrderGet(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));

    public IReadOnlyExternalEntityContext<MonzoPotsResponse> MonzoPots =>
        new ReadOnlyExternalEntityContext<MonzoPotsResponse>(
            new MonzoPotGet(
                _sharedContext.Instrumentation,
                _sharedContext.ApiVariantMapper,
                new ConsentAccessTokenGet(
                    _sharedContext.DbService.GetDbMethods(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.EncryptionKeyInfo,
                    new AccountAccessConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticPaymentConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods()),
                    new DomesticVrpConsentCommon(
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsent>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                        _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                        _sharedContext.Instrumentation,
                        _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                        _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                        _sharedContext.DbService.GetDbMethods())),
                new AccountAccessConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentPersisted>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<AccountAccessConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                _sharedContext.BankProfileService,
                _sharedContext.ObWacCertificateMethods,
                _sharedContext.ObSealCertificateMethods));
}
