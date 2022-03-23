// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
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
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;
using DomesticPaymentConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public interface IDomesticPaymentConsentsContext :
        IEntityContext<DomesticPaymentConsentRequest,
            IDomesticPaymentConsentPublicQuery,
            DomesticPaymentConsentResponse>,
        IReadFundsConfirmationContext<DomesticPaymentConsentResponse>
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
            DomesticPaymentConsentResponse>,
        IReadFundsConfirmationContextInternal<DomesticPaymentConsentResponse> { }

    internal class DomesticPaymentConsentsConsentContext :
        ObjectContextBase<DomesticPaymentConsent>,
        IDomesticPaymentConsentsContextInternal
    {
        private readonly ISharedContext _sharedContext;

        public DomesticPaymentConsentsConsentContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            PostObject = new DomesticPaymentConsentPost(
                sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                sharedContext.DbService.GetDbSaveChangesMethodClass(),
                sharedContext.TimeProvider,
                sharedContext.SoftwareStatementProfileCachedRepo,
                sharedContext.Instrumentation,
                sharedContext.ApiVariantMapper,
                sharedContext.DbService.GetDbEntityMethodsClass<BankApiSetPersisted>(),
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
                    DomesticPaymentConsentResponse>(
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
                new DomesticPaymentConsentAuthContext(
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsentAuthContextPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticPaymentConsent>(),
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation));

        public IObjectRead<DomesticPaymentConsentResponse> ReadObject { get; }

        public IObjectRead<DomesticPaymentConsentResponse> ReadFundsConfirmationObject { get; }

        public IObjectPost<DomesticPaymentConsentRequest, DomesticPaymentConsentResponse> PostObject { get; }

        public IObjectReadLocal<IDomesticPaymentConsentPublicQuery, DomesticPaymentConsentResponse> ReadLocalObject
        {
            get;
        }
    }
}
