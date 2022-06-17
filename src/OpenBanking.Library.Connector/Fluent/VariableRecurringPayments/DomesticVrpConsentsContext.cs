﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;
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


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IDomesticVrpConsentsContext :
        IConsentContext<DomesticVrpConsentRequest,
            IDomesticVrpConsentPublicQuery,
            DomesticVrpConsentResponse, DomesticVrpConsentBaseResponse>,
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
        IConsentContextInternal<DomesticVrpConsentRequest,
            IDomesticVrpConsentPublicQuery,
            DomesticVrpConsentResponse, DomesticVrpConsentBaseResponse>,
        IReadFundsConfirmationContextInternal<DomesticVrpConsentReadFundsConfirmationResponse>,
        IDeleteConsentContextInternal { }

    internal class DomesticVrpConsentsContext :
        IDomesticVrpConsentsContextInternal
    {
        private readonly ISharedContext _sharedContext;

        public DomesticVrpConsentsContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
            CreateObject = new DomesticVrpConsentPost(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                new GrantPost(_sharedContext.ApiClient),
                sharedContext.DbService.GetDbEntityMethodsClass<VariableRecurringPaymentsApiEntity>(),
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>());
            ReadObject = new DomesticVrpConsentGet(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                new GrantPost(_sharedContext.ApiClient));
            ReadFundsConfirmationObject = new DomesticVrpConsentGetFundsConfirmation(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                new GrantPost(_sharedContext.ApiClient),
                new AuthContextAccessTokenGet(
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(_sharedContext.ApiClient)));
            ReadLocalObject =
                new LocalEntityRead<DomesticVrpConsentPersisted, IDomesticVrpConsentPublicQuery,
                    DomesticVrpConsentBaseResponse>(
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
                    sharedContext.Instrumentation,
                    sharedContext.BankProfileDefinitions,
                    new GrantPost(_sharedContext.ApiClient));
        }

        public IObjectRead<DomesticVrpConsentResponse, ConsentReadParams> ReadObject { get; }

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

        public IObjectReadWithSearch<IDomesticVrpConsentPublicQuery, DomesticVrpConsentBaseResponse, LocalReadParams>
            ReadLocalObject { get; }

        public IObjectCreate<DomesticVrpConsentRequest, DomesticVrpConsentResponse, ConsentCreateParams> CreateObject
        {
            get;
        }

        public IObjectRead<DomesticVrpConsentReadFundsConfirmationResponse, ConsentBaseReadParams>
            ReadFundsConfirmationObject { get; }

        public IObjectDelete<ConsentDeleteParams> DeleteObject { get; }
    }
}
