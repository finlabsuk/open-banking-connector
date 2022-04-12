// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using BankApiSetPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankApiSet;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IDomesticVrpConsentsContext :
        IEntityContext<DomesticVrpConsentRequest,
            IDomesticVrpConsentPublicQuery,
            DomesticVrpConsentReadResponse, DomesticVrpConsentReadLocalResponse>,
        IReadFundsConfirmationContext<DomesticVrpConsentReadFundsConfirmationResponse>,
        IDeleteContext
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        ILocalEntityContext<DomesticVrpConsentAuthContextRequest,
                IDomesticVrpConsentAuthContextPublicQuery,
                DomesticVrpConsentAuthContextCreateLocalResponse,
                DomesticVrpConsentAuthContextReadLocalResponse>
            AuthContexts { get; }
    }

    internal interface IDomesticVrpConsentsContextInternal :
        IDomesticVrpConsentsContext,
        IEntityContextInternal<DomesticVrpConsentRequest,
            IDomesticVrpConsentPublicQuery,
            DomesticVrpConsentReadResponse, DomesticVrpConsentReadLocalResponse>,
        IReadFundsConfirmationContextInternal<DomesticVrpConsentReadFundsConfirmationResponse>,
        IDeleteContextInternal { }

    internal class DomesticVrpConsentsContext :
        ObjectContextBase<DomesticVrpConsentPersisted>, IDomesticVrpConsentsContextInternal
    {
        private readonly ISharedContext _sharedContext;

        public DomesticVrpConsentsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            CreateObject = new DomesticVrpConsentPost(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                sharedContext.DbService.GetDbEntityMethodsClass<BankApiSetPersisted>(),
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>());
            ReadObject = new DomesticVrpConsentGet(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper);
            ReadFundsConfirmationObject = new DomesticVrpConsentGetFundsConfirmation(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper);
            ReadLocalObject =
                new LocalEntityGet<DomesticVrpConsentPersisted, IDomesticVrpConsentPublicQuery,
                    DomesticVrpConsentReadLocalResponse>(
                    sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation);
            DeleteObject =
                new DomesticVrpConsentDelete(
                    sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation);
        }

        public IObjectRead<DomesticVrpConsentReadResponse> ReadObject { get; }

        public ILocalEntityContext<DomesticVrpConsentAuthContextRequest,
            IDomesticVrpConsentAuthContextPublicQuery,
            DomesticVrpConsentAuthContextCreateLocalResponse,
            DomesticVrpConsentAuthContextReadLocalResponse> AuthContexts =>
            new LocalEntityContextInternal<DomesticVrpConsentAuthContextPersisted,
                DomesticVrpConsentAuthContextRequest,
                IDomesticVrpConsentAuthContextPublicQuery,
                DomesticVrpConsentAuthContextCreateLocalResponse,
                DomesticVrpConsentAuthContextReadLocalResponse>(
                _sharedContext,
                new DomesticVrpConsentAuthContext(
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentAuthContextPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation));

        public IObjectReadLocal<IDomesticVrpConsentPublicQuery, DomesticVrpConsentReadLocalResponse> ReadLocalObject
        {
            get;
        }

        public IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentReadResponse> CreateObject { get; }
        public IObjectRead<DomesticVrpConsentReadFundsConfirmationResponse> ReadFundsConfirmationObject { get; }
        public IObjectDelete DeleteObject { get; }
    }
}
