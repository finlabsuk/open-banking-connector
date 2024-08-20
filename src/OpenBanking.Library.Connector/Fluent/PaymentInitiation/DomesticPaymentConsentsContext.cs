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
    IReadFundsConfirmationContext<DomesticPaymentConsentFundsConfirmationResponse, ConsentBaseReadParams>
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

internal class DomesticPaymentConsentsConsentContext :
    IDomesticPaymentConsentsContext
{
    private readonly DomesticPaymentConsentOperations _domesticPaymentConsentOperations;
    private readonly ISharedContext _sharedContext;

    public DomesticPaymentConsentsConsentContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
        var clientAccessTokenGet = new ClientAccessTokenGet(
            sharedContext.TimeProvider,
            new GrantPost(
                sharedContext.ApiClient,
                sharedContext.Instrumentation,
                sharedContext.MemoryCache,
                sharedContext.TimeProvider),
            sharedContext.Instrumentation,
            sharedContext.MemoryCache,
            sharedContext.EncryptionKeyInfo);
        _domesticPaymentConsentOperations = new DomesticPaymentConsentOperations(
            sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation,
            sharedContext.ApiVariantMapper,
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
            _sharedContext.ObSealCertificateMethods,
            clientAccessTokenGet);
        CreateObject = _domesticPaymentConsentOperations;
        ReadObject = _domesticPaymentConsentOperations;
        DeleteLocalObject = new LocalEntityDelete<DomesticPaymentConsent, LocalDeleteParams>(
            sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation);
    }

    public ILocalEntityContext<DomesticPaymentConsentAuthContextRequest,
        IDomesticPaymentConsentAuthContextPublicQuery,
        DomesticPaymentConsentAuthContextCreateResponse,
        DomesticPaymentConsentAuthContextReadResponse> AuthContexts =>
        new LocalEntityContext<DomesticPaymentConsentAuthContextPersisted,
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
                _sharedContext.Instrumentation,
                _sharedContext.BankProfileService,
                _sharedContext.ObSealCertificateMethods));

    public IObjectRead<DomesticPaymentConsentCreateResponse, ConsentReadParams> ReadObject { get; }


    public IObjectCreate<DomesticPaymentConsentRequest, DomesticPaymentConsentCreateResponse, ConsentCreateParams>
        CreateObject { get; }

    public IObjectDelete<LocalDeleteParams> DeleteLocalObject { get; }

    public Task<DomesticPaymentConsentFundsConfirmationResponse> ReadFundsConfirmationAsync(
        ConsentBaseReadParams readParams) => _domesticPaymentConsentOperations.ReadFundsConfirmationAsync(readParams);
}
