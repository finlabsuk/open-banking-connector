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
        ///     API for DomesticPaymentConsent object which corresponds to user consent for a domestic payment.
        /// </summary>
        IDomesticPaymentConsentsContext DomesticPaymentConsents { get; }

        /// <summary>
        ///     API for DomesticPayment which corresponds to a domestic payment.
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
