// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using DomesticVrpPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrp;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;


namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IVariableRecurringPaymentsContext
    {
        /// <summary>
        ///     API for DomesticVrpConsent objects.
        ///     A DomesticVrpConsent is an Open Banking object and corresponds to an "intended" domestic variable recurring
        ///     payment.
        /// </summary>
        IDomesticVrpConsentsContext DomesticVrpConsents { get; }

        /// <summary>
        ///     API for DomesticVrp objects.
        ///     A DomesticVrp corresponds to a domestic variable recurring payment and may be created once a DomesticVrpConsent is
        ///     approved by a user.
        /// </summary>
        IExternalEntityContext<DomesticVrpRequest, DomesticVrpResponse> DomesticVrps { get; }
    }

    internal class VariableRecurringPaymentsContext : IVariableRecurringPaymentsContext
    {
        private readonly ISharedContext _sharedContext;

        public VariableRecurringPaymentsContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IDomesticVrpConsentsContext DomesticVrpConsents =>
            new DomesticVrpConsentsContext(_sharedContext);

        public IExternalEntityContext<DomesticVrpRequest, DomesticVrpResponse> DomesticVrps =>
            new ExternalEntityContextInternal<DomesticVrpRequest, DomesticVrpResponse, IDomesticVrpPublicQuery>(
                _sharedContext,
                new DomesticVrpPost(
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation,
                    _sharedContext.ApiVariantMapper),
                new DomesticVrpGet(
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpPersisted>(),
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.Instrumentation,
                    _sharedContext.ApiVariantMapper));
    }
}
