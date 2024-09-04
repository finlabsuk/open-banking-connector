// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
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

internal class DomesticVrpConsentsContext :
    IDomesticVrpConsentsContext
{
    private readonly DomesticVrpConsentOperations _domesticVrpConsentOperations;
    private readonly ISharedContext _sharedContext;

    public DomesticVrpConsentsContext(ISharedContext sharedContext)
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
        _domesticVrpConsentOperations = new DomesticVrpConsentOperations(
            sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
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
            clientAccessTokenGet,
            new DomesticVrpConsentCommon(
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentAccessToken>(),
                _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentRefreshToken>(),
                _sharedContext.Instrumentation));
        CreateObject = _domesticVrpConsentOperations;
        ReadObject = _domesticVrpConsentOperations;
        DeleteObject =
            new DomesticVrpConsentDelete(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.Instrumentation,
                sharedContext.BankProfileService,
                sharedContext.ObWacCertificateMethods,
                sharedContext.ObSealCertificateMethods,
                clientAccessTokenGet);
    }

    public IObjectRead<DomesticVrpConsentCreateResponse, ConsentReadParams> ReadObject { get; }

    public ILocalEntityContext<DomesticVrpConsentAuthContextRequest,
        IDomesticVrpConsentAuthContextPublicQuery,
        DomesticVrpConsentAuthContextCreateResponse,
        DomesticVrpConsentAuthContextReadResponse> AuthContexts =>
        new LocalEntityContext<DomesticVrpConsentAuthContextPersisted,
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
        VrpConsentFundsConfirmationCreateParams createParams) =>
        _domesticVrpConsentOperations.CreateFundsConfirmationAsync(createParams);
}
