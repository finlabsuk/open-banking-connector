// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;

public interface IDomesticPaymentConsentsContext :
    IConsentContext<DomesticPaymentConsentRequest, DomesticPaymentConsentCreateResponse,
        DomesticPaymentConsentCreateResponse>,
    IDeleteLocalContext,
    IReadFundsConfirmationContext<DomesticPaymentConsentReadFundsConfirmationResponse>
{
    /// <summary>
    ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
    ///     authorisation of consent.
    /// </summary>
    ILocalEntityContext<DomesticPaymentConsentAuthContextRequest,
            IDomesticPaymentConsentAuthContextPublicQuery,
            DomesticPaymentConsentAuthContextCreateResponse,
            DomesticPaymentConsentAuthContextReadResponse>
        AuthContexts { get; }
}

internal interface IDomesticPaymentConsentsContextInternal :
    IDomesticPaymentConsentsContext,
    IConsentContextInternal<DomesticPaymentConsentRequest, DomesticPaymentConsentCreateResponse,
        DomesticPaymentConsentCreateResponse>,
    IDeleteLocalContextInternal,
    IReadFundsConfirmationContextInternal<DomesticPaymentConsentReadFundsConfirmationResponse> { }

internal class DomesticPaymentConsentsConsentContext :
    IDomesticPaymentConsentsContextInternal
{
    private readonly ISharedContext _sharedContext;

    public DomesticPaymentConsentsConsentContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
        var domesticPaymentConsentOperations = new DomesticPaymentConsentOperations(
            sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
            sharedContext.SoftwareStatementProfileCachedRepo,
            sharedContext.Instrumentation,
            sharedContext.ApiVariantMapper,
            new GrantPost(
                _sharedContext.ApiClient,
                _sharedContext.Instrumentation,
                _sharedContext.MemoryCache,
                _sharedContext.TimeProvider),
            sharedContext.BankProfileService,
            new ConsentAccessTokenGet(
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                new GrantPost(
                    _sharedContext.ApiClient,
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.TimeProvider),
                _sharedContext.Instrumentation,
                _sharedContext.MemoryCache,
                _sharedContext.EncryptionKeyInfo),
            sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationEntity>(),
            _sharedContext.ObWacCertificateMethods,
            _sharedContext.ObSealCertificateMethods);
        CreateObject = domesticPaymentConsentOperations;
        ReadObject = domesticPaymentConsentOperations;
        ReadFundsConfirmationObject = domesticPaymentConsentOperations;
        DeleteLocalObject = new LocalEntityDelete<DomesticPaymentConsent, LocalDeleteParams>(
            sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
            sharedContext.SoftwareStatementProfileCachedRepo,
            sharedContext.Instrumentation);
    }

    public ILocalEntityContext<DomesticPaymentConsentAuthContextRequest,
        IDomesticPaymentConsentAuthContextPublicQuery,
        DomesticPaymentConsentAuthContextCreateResponse,
        DomesticPaymentConsentAuthContextReadResponse> AuthContexts =>
        new LocalEntityContextInternal<DomesticPaymentConsentAuthContextPersisted,
            DomesticPaymentConsentAuthContextRequest,
            IDomesticPaymentConsentAuthContextPublicQuery,
            DomesticPaymentConsentAuthContextCreateResponse,
            DomesticPaymentConsentAuthContextReadResponse>(
            _sharedContext,
            new DomesticPaymentConsentAuthContextPost(
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsentAuthContextPersisted>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation,
                _sharedContext.BankProfileService));

    public IObjectRead<DomesticPaymentConsentCreateResponse, ConsentReadParams> ReadObject { get; }

    public IObjectReadFundsConfirmation<DomesticPaymentConsentReadFundsConfirmationResponse, ConsentBaseReadParams>
        ReadFundsConfirmationObject { get; }

    public IObjectCreate<DomesticPaymentConsentRequest, DomesticPaymentConsentCreateResponse, ConsentCreateParams>
        CreateObject { get; }

    public IObjectDelete<LocalDeleteParams> DeleteLocalObject { get; }
}
