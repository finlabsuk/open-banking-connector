// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using DomesticPaymentOperations =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation.DomesticPayment;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

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
                _sharedContext.EncryptionKeyInfo),
            _sharedContext.BankProfileService,
            _sharedContext.ObWacCertificateMethods,
            _sharedContext.ObSealCertificateMethods,
            clientAccessTokenGet,
            new DomesticPaymentConsentCommon(
                _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentPersisted>(),
                _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentAccessToken>(),
                _sharedContext.DbService.GetDbEntityMethods<DomesticPaymentConsentRefreshToken>(),
                _sharedContext.Instrumentation));
    }

    public IDomesticPaymentConsentsContext DomesticPaymentConsents =>
        new DomesticPaymentConsentsConsentContext(_sharedContext);

    public IDomesticPaymentContext<DomesticPaymentRequest, DomesticPaymentResponse,
            DomesticPaymentPaymentDetailsResponse, ConsentExternalCreateParams,
            ConsentExternalEntityReadParams>
        DomesticPayments =>
        _domesticPayments;
}
