// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpOperations =
    FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments.DomesticVrp;


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

        public IExternalEntityContext<DomesticVrpRequest, DomesticVrpResponse> DomesticVrps
        {
            get
            {
                var domesticVrp = new DomesticVrpOperations(
                    _sharedContext.DbService.GetDbEntityMethodsClass<DomesticVrpConsentPersisted>(),
                    _sharedContext.Instrumentation,
                    _sharedContext.SoftwareStatementProfileCachedRepo,
                    _sharedContext.ApiVariantMapper,
                    _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    _sharedContext.TimeProvider,
                    new GrantPost(_sharedContext.ApiClient),
                    new AuthContextAccessTokenGet(
                        _sharedContext.SoftwareStatementProfileCachedRepo,
                        _sharedContext.DbService.GetDbSaveChangesMethodClass(),
                        _sharedContext.TimeProvider,
                        new GrantPost(_sharedContext.ApiClient)));
                return new ExternalEntityContextInternal<DomesticVrpRequest, DomesticVrpResponse>(
                    domesticVrp,
                    domesticVrp);
            }
        }
    }
}
