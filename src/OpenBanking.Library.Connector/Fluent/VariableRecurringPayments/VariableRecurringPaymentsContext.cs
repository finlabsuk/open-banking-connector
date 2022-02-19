// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;

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
        IDomesticVrpsContext DomesticVrps { get; }
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

        public IDomesticVrpsContext DomesticVrps =>
            new DomesticVrpsContext(_sharedContext);
    }
}
