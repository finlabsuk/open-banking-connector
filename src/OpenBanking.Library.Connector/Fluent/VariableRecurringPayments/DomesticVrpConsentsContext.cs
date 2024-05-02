// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments;

public interface IDomesticVrpConsentsContext :
    IConsentContext<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, DomesticVrpConsentCreateResponse>,
    ICreateVrpConsentFundsConfirmationContext<DomesticVrpConsentFundsConfirmationRequest,
        DomesticVrpConsentFundsConfirmationResponse>,
    IDeleteConsentContext
{
    /// <summary>
    ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
    ///     authorisation of consent.
    /// </summary>
    ILocalEntityContext<DomesticVrpConsentAuthContextRequest,
            IDomesticVrpConsentAuthContextPublicQuery,
            DomesticVrpConsentAuthContextCreateResponse,
            DomesticVrpConsentAuthContextReadResponse>
        AuthContexts { get; }
}

internal interface IDomesticVrpConsentsContextInternal :
    IDomesticVrpConsentsContext,
    IConsentContextInternal<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse,
        DomesticVrpConsentCreateResponse>,
    IDeleteConsentContextInternal { }

internal class DomesticVrpConsentsContext :
    IDomesticVrpConsentsContextInternal
{
    private readonly DomesticVrpConsentOperations _domesticVrpConsentOperations;
    private readonly ISharedContext _sharedContext;

    public DomesticVrpConsentsContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
        _domesticVrpConsentOperations = new DomesticVrpConsentOperations(
            sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
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
        CreateObject = _domesticVrpConsentOperations;
        ReadObject = _domesticVrpConsentOperations;
        DeleteObject =
            new DomesticVrpConsentDelete(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.Instrumentation,
                sharedContext.BankProfileService,
                new GrantPost(
                    _sharedContext.ApiClient,
                    _sharedContext.Instrumentation,
                    _sharedContext.MemoryCache,
                    _sharedContext.TimeProvider),
                sharedContext.ObWacCertificateMethods,
                sharedContext.ObSealCertificateMethods);
    }

    public IObjectRead<DomesticVrpConsentCreateResponse, ConsentReadParams> ReadObject { get; }

    public ILocalEntityContext<DomesticVrpConsentAuthContextRequest,
        IDomesticVrpConsentAuthContextPublicQuery,
        DomesticVrpConsentAuthContextCreateResponse,
        DomesticVrpConsentAuthContextReadResponse> AuthContexts =>
        new LocalEntityContextInternal<DomesticVrpConsentAuthContextPersisted,
            DomesticVrpConsentAuthContextRequest,
            IDomesticVrpConsentAuthContextPublicQuery,
            DomesticVrpConsentAuthContextCreateResponse,
            DomesticVrpConsentAuthContextReadResponse>(
            _sharedContext,
            new DomesticVrpConsentAuthContextPost(
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentAuthContextPersisted>(),
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                _sharedContext.Instrumentation,
                _sharedContext.BankProfileService,
                _sharedContext.ObSealCertificateMethods));

    public IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, ConsentCreateParams>
        CreateObject { get; }

    public IObjectDelete<ConsentDeleteParams> DeleteObject { get; }

    public Task<DomesticVrpConsentFundsConfirmationResponse> CreateFundsConfirmationAsync(
        DomesticVrpConsentFundsConfirmationRequest request,
        VrpConsentFundsConfirmationCreateParams createParams) =>
        _domesticVrpConsentOperations.CreateFundsConfirmationAsync(request, createParams);
}
