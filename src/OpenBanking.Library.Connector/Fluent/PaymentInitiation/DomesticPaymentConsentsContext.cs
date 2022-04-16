// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public interface IDomesticPaymentConsentsContext :
        IEntityContext<DomesticPaymentConsentRequest,
            IDomesticPaymentConsentPublicQuery,
            DomesticPaymentConsentReadResponse, DomesticPaymentConsentReadLocalResponse>,
        IReadFundsConfirmationContext<DomesticPaymentConsentReadFundsConfirmationResponse>
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        ILocalEntityContext<DomesticPaymentConsentAuthContextRequest,
                IDomesticPaymentConsentAuthContextPublicQuery,
                DomesticPaymentConsentAuthContextCreateLocalResponse,
                DomesticPaymentConsentAuthContextReadLocalResponse>
            AuthContexts { get; }
    }

    internal interface IDomesticPaymentConsentsContextInternal :
        IDomesticPaymentConsentsContext,
        IEntityContextInternal<DomesticPaymentConsentRequest,
            IDomesticPaymentConsentPublicQuery,
            DomesticPaymentConsentReadResponse, DomesticPaymentConsentReadLocalResponse>,
        IReadFundsConfirmationContextInternal<DomesticPaymentConsentReadFundsConfirmationResponse> { }

    internal class DomesticPaymentConsentsConsentContext :
        ObjectContextBase<DomesticPaymentConsent>,
        IDomesticPaymentConsentsContextInternal
    {
        private readonly ISharedContext _sharedContext;

        public DomesticPaymentConsentsConsentContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            CreateObject = new DomesticPaymentConsentPost(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                sharedContext.DbService.GetDbEntityMethodsClass<PaymentInitiationApiEntity>(),
                sharedContext.DbService.GetDbEntityMethodsClass<BankRegistrationPersisted>());
            ReadObject = new DomesticPaymentConsentGet(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper);
            ReadFundsConfirmationObject = new DomesticPaymentConsentGetFundsConfirmation(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper);
            ReadLocalObject =
                new LocalEntityGet<DomesticPaymentConsent, IDomesticPaymentConsentPublicQuery,
                    DomesticPaymentConsentReadLocalResponse>(
                    sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation);
        }

        public ILocalEntityContext<DomesticPaymentConsentAuthContextRequest,
            IDomesticPaymentConsentAuthContextPublicQuery,
            DomesticPaymentConsentAuthContextCreateLocalResponse,
            DomesticPaymentConsentAuthContextReadLocalResponse> AuthContexts =>
            new LocalEntityContextInternal<DomesticPaymentConsentAuthContextPersisted,
                DomesticPaymentConsentAuthContextRequest,
                IDomesticPaymentConsentAuthContextPublicQuery,
                DomesticPaymentConsentAuthContextCreateLocalResponse,
                DomesticPaymentConsentAuthContextReadLocalResponse>(
                _sharedContext,
                new DomesticPaymentConsentAuthContextPost(
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsentAuthContextPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation));

        public IObjectRead<DomesticPaymentConsentReadResponse> ReadObject { get; }

        public IObjectRead<DomesticPaymentConsentReadFundsConfirmationResponse> ReadFundsConfirmationObject { get; }

        public IObjectCreate<DomesticPaymentConsentRequest, DomesticPaymentConsentReadResponse> CreateObject { get; }

        public IObjectReadLocal<IDomesticPaymentConsentPublicQuery, DomesticPaymentConsentReadLocalResponse>
            ReadLocalObject { get; }
    }
}
