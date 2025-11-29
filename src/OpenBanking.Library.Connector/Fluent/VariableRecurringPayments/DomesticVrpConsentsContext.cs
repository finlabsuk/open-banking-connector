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
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments;

public interface IDomesticVrpConsentsContext :
    IConsentContext<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, DomesticVrpConsentCreateResponse>,
    ICreateVrpConsentFundsConfirmationContext,
    IUpdateVrpConsentContext,
    IDeleteConsentContext
{
    /// <summary>
    ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
    ///     authorisation of consent.
    /// </summary>
    IDomesticVrpConsentAuthContextsContext
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
            sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentPersisted>(),
            sharedContext.DbService.GetDbMethods(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation,
            sharedContext.ApiVariantMapper,
            sharedContext.BankProfileService,
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
                _sharedContext.EncryptionKeyInfo),
            new ConsentCommon(
                _sharedContext.DbService.GetDbEntityMethods<BankRegistrationEntity>(),
                _sharedContext.Instrumentation,
                _sharedContext.DbService.GetDbMethods(),
                _sharedContext.DbService.GetDbEntityMethods<ExternalApiSecretEntity>(),
                _sharedContext.DbService.GetDbEntityMethods<SoftwareStatementEntity>()),
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
        CreateObject = _domesticVrpConsentOperations;
        ReadObject = _domesticVrpConsentOperations;
        DeleteObject =
            new DomesticVrpConsentDelete(
                sharedContext.DbService.GetDbEntityMethods<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbMethods(),
                sharedContext.TimeProvider,
                sharedContext.Instrumentation,
                sharedContext.BankProfileService,
                sharedContext.ObWacCertificateMethods,
                sharedContext.ObSealCertificateMethods,
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
        AuthContexts = new DomesticVrpConsentAuthContextsContext(_sharedContext);
    }

    public IObjectRead<DomesticVrpConsentCreateResponse, ConsentReadParams> ReadObject { get; }


    public IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, ConsentCreateParams>
        CreateObject { get; }

    public IObjectDelete<ConsentDeleteParams> DeleteObject { get; }

    public Task<DomesticVrpConsentFundsConfirmationResponse> CreateFundsConfirmationAsync(
        VrpConsentFundsConfirmationCreateParams createParams) =>
        _domesticVrpConsentOperations.CreateFundsConfirmationAsync(createParams);

    public IDomesticVrpConsentAuthContextsContext AuthContexts { get; }

    public Task<DomesticVrpConsentCreateResponse> UpdateAsync(
        DomesticVrpConsentRequest request,
        ConsentBaseReadParams updateParams) =>
        _domesticVrpConsentOperations.UpdateAsync(request, updateParams);
}
