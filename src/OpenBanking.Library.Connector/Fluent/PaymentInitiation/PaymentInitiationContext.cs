// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
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
    IExternalEntityContext<DomesticPaymentRequest, DomesticPaymentResponse> DomesticPayments { get; }
}

internal class PaymentInitiationContext : IPaymentInitiationContext
{
    private readonly ISharedContext _sharedContext;

    public PaymentInitiationContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
    }

    public IDomesticPaymentConsentsContext DomesticPaymentConsents =>
        new DomesticPaymentConsentsConsentContext(_sharedContext);

    public IExternalEntityContext<DomesticPaymentRequest, DomesticPaymentResponse> DomesticPayments
    {
        get
        {
            var domesticPayment = new DomesticPaymentOperations(
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsentPersisted>(),
                _sharedContext.Instrumentation,
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.ApiVariantMapper,
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                new GrantPost(
                    _sharedContext.ApiClient,
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.TimeProvider),
                new ConsentAccessTokenGet(
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(
                        _sharedContext.ApiClient,
                        _sharedContext.Instrumentation,
                        _sharedContext.MemoryCache,
                        _sharedContext.TimeProvider),
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache),
                _sharedContext.BankProfileService);
            return new ExternalEntityContextInternal<DomesticPaymentRequest, DomesticPaymentResponse>(
                domesticPayment,
                domesticPayment);
        }
    }
}
