// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using DomesticPaymentOperations =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation.DomesticPayment;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;

public interface IPaymentInitiationContext
{
    /// <summary>
    ///     API for DomesticPaymentConsent objects.
    ///     A DomesticPaymentConsent is an Open Banking object and corresponds to an "intended" domestic payment.
    /// </summary>
    IDomesticPaymentConsentsContext DomesticPaymentConsents { get; }

    /// <summary>
    ///     API for DomesticPayment objects.
    ///     A DomesticPayment corresponds to a domestic payment and may be created once a DomesticPaymentConsent is approved by
    ///     a user.
    /// </summary>
    IDomesticPaymentContext<DomesticPaymentRequest, DomesticPaymentResponse, DomesticPaymentPaymentDetailsResponse,
            ConsentExternalCreateParams,
            ConsentExternalEntityReadParams>
        DomesticPayments { get; }
}

internal class PaymentInitiationContext : IPaymentInitiationContext
{
    private readonly DomesticPaymentOperations _domesticPayments;
    private readonly ISharedContext _sharedContext;

    public PaymentInitiationContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
        var grantPost = new GrantPost(
            _sharedContext.ApiClient,
            _sharedContext.Instrumentation,
            _sharedContext.MemoryCache,
            _sharedContext.TimeProvider);
        var clientAccessTokenGet = new ClientAccessTokenGet(
            sharedContext.TimeProvider,
            grantPost,
            sharedContext.Instrumentation,
            sharedContext.MemoryCache,
            sharedContext.EncryptionKeyInfo);
        _domesticPayments = new DomesticPaymentOperations(
            _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentPersisted>(),
            _sharedContext.Instrumentation,
            _sharedContext.ApiVariantMapper,
            _sharedContext.TimeProvider,
            new ConsentAccessTokenGet(
                _sharedContext.DbService.GetDbMethods(),
                _sharedContext.TimeProvider,
                grantPost,
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
                    _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentPersisted>(),
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
            _sharedContext.BankProfileService,
            _sharedContext.ObWacCertificateMethods,
            _sharedContext.ObSealCertificateMethods,
            clientAccessTokenGet,
            new DomesticPaymentConsentCommon(
                _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentPersisted>(),
                _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                _sharedContext.Instrumentation,
                _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                _sharedContext.DbService.GetDbMethods()));
    }

    public IDomesticPaymentConsentsContext DomesticPaymentConsents =>
        new DomesticPaymentConsentsConsentContext(_sharedContext);

    public IDomesticPaymentContext<DomesticPaymentRequest, DomesticPaymentResponse,
            DomesticPaymentPaymentDetailsResponse, ConsentExternalCreateParams,
            ConsentExternalEntityReadParams>
        DomesticPayments =>
        _domesticPayments;
}
