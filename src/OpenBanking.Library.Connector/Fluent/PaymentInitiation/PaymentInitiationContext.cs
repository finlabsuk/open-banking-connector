// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public interface IPaymentInitiationContext
    {
        /// <summary>
        ///     API for DomesticPaymentConsent objects.
        ///     A DomesticPaymentConsent is an Open Banking object and corresponds to an "intended" domestic payment.
        /// </summary>
        IDomesticPaymentConsentsContext DomesticPaymentConsents { get; }

        /// <summary>
        ///     API for DomesticPayment objects.
        ///     A DomesticPayment corresponds to a domestic payment and may be created once a DomesticPaymentConsent is approved by
        ///     a user.
        /// </summary>
        IDomesticPaymentsContext DomesticPayments { get; }
    }

    internal class PaymentInitiationContext : IPaymentInitiationContext
    {
        private readonly ISharedContext _sharedContext;

        public PaymentInitiationContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IDomesticPaymentConsentsContext DomesticPaymentConsents =>
            new DomesticPaymentConsentsConsentContext(_sharedContext);

        public IDomesticPaymentsContext DomesticPayments =>
            new DomesticPaymentsContext(_sharedContext);
    }
}
