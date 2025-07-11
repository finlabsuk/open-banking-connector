﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpOperations =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments.DomesticVrp;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments;

public interface IVariableRecurringPaymentsContext
{
    /// <summary>
    ///     API for DomesticVrpConsent objects.
    ///     A DomesticVrpConsent is an Open Banking object and corresponds to an "intended" domestic variable recurring
    ///     payment.
    /// </summary>
    IDomesticVrpConsentsContext DomesticVrpConsents { get; }

    /// <summary>
    ///     API for DomesticVrp objects.
    ///     A DomesticVrp corresponds to a domestic variable recurring payment and may be created once a DomesticVrpConsent is
    ///     approved by a user.
    /// </summary>
    IDomesticVrpContext<DomesticVrpRequest, DomesticVrpResponse, DomesticVrpPaymentDetailsResponse,
        ConsentExternalCreateParams,
        ConsentExternalEntityReadParams> DomesticVrps { get; }
}

internal class VariableRecurringPaymentsContext : IVariableRecurringPaymentsContext
{
    private readonly DomesticVrpOperations _domesticVrp;
    private readonly ISharedContext _sharedContext;

    public VariableRecurringPaymentsContext(ISharedContext sharedContext)
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
        _domesticVrp = new DomesticVrpOperations(
            _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentPersisted>(),
            _sharedContext.Instrumentation,
            _sharedContext.ApiVariantMapper,
            _sharedContext.DbService.GetDbMethods(),
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
                    _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsent>(),
                    _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                    _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                    _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                    _sharedContext.DbService.GetDbMethods()),
                new DomesticVrpConsentCommon(
                    _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentPersisted>(),
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
            new DomesticVrpConsentCommon(
                _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentPersisted>(),
                _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentAccessToken>(),
                _sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentRefreshToken>(),
                _sharedContext.Instrumentation,
                _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>(),
                _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                _sharedContext.DbService.GetDbMethods()));
    }

    public IDomesticVrpConsentsContext DomesticVrpConsents =>
        new DomesticVrpConsentsContext(_sharedContext);

    public IDomesticVrpContext<DomesticVrpRequest, DomesticVrpResponse, DomesticVrpPaymentDetailsResponse,
            ConsentExternalCreateParams,
            ConsentExternalEntityReadParams>
        DomesticVrps => _domesticVrp;
}
