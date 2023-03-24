// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
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
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments;

public interface IDomesticVrpConsentsContext :
    IConsentContext<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, DomesticVrpConsentCreateResponse>,
    IReadFundsConfirmationContext<DomesticVrpConsentReadFundsConfirmationResponse>,
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
    IReadFundsConfirmationContextInternal<DomesticVrpConsentReadFundsConfirmationResponse>,
    IDeleteConsentContextInternal { }

internal class DomesticVrpConsentsContext :
    IDomesticVrpConsentsContextInternal
{
    private readonly ISharedContext _sharedContext;

    public DomesticVrpConsentsContext(ISharedContext sharedContext)
    {
        _sharedContext = sharedContext;
        var domesticVrpConsentOperations = new DomesticVrpConsentOperations(
            sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
            sharedContext.DbService.GetDbSaveChangesMethodClass(),
            sharedContext.TimeProvider,
            sharedContext.SoftwareStatementProfileCachedRepo,
            sharedContext.Instrumentation,
            sharedContext.ApiVariantMapper,
            new GrantPost(_sharedContext.ApiClient),
            sharedContext.BankProfileService,
            new ConsentAccessTokenGet(
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                _sharedContext.TimeProvider,
                new GrantPost(_sharedContext.ApiClient),
                _sharedContext.Instrumentation),
            sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>());
        CreateObject = domesticVrpConsentOperations;
        ReadObject = domesticVrpConsentOperations;
        ReadFundsConfirmationObject = domesticVrpConsentOperations;
        DeleteObject =
            new DomesticVrpConsentDelete(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.BankProfileService,
                new GrantPost(_sharedContext.ApiClient));
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
                _sharedContext.SoftwareStatementProfileCachedRepo,
                _sharedContext.Instrumentation));

    public IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentCreateResponse, ConsentCreateParams>
        CreateObject { get; }

    public IObjectReadFundsConfirmation<DomesticVrpConsentReadFundsConfirmationResponse, ConsentBaseReadParams>
        ReadFundsConfirmationObject { get; }

    public IObjectDelete<ConsentDeleteParams> DeleteObject { get; }
}
