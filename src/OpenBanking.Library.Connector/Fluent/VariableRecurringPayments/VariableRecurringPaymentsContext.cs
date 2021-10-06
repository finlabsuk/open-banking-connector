// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IVariableRecurringPaymentsContext
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        IAuthContextsContext AuthContexts { get; }

        /// <summary>
        ///     API for DomesticVrpConsent object which corresponds to user consent for a domestic VRP.
        /// </summary>
        IDomesticVrpConsentsContext DomesticVrpConsents { get; }

        /// <summary>
        ///     API for DomesticVrp object which corresponds to domestic VRP.
        /// </summary>
        IDomesticVrpsContext DomesticVrps { get; }

        /// <summary>
        ///     API for DomesticPayment which corresponds to a domestic payment.
        /// </summary>
        IEntityContext<DomesticPaymentRequest, IDomesticPaymentPublicQuery, DomesticPaymentResponse> DomesticPayments
        {
            get;
        }
    }

    internal class VariableRecurringPaymentsContext : IVariableRecurringPaymentsContext
    {
        private readonly ISharedContext _sharedContext;

        public VariableRecurringPaymentsContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IAuthContextsContext AuthContexts => new AuthContextsContext(_sharedContext);

        public IDomesticVrpConsentsContext DomesticVrpConsents =>
            new DomesticVrpConsentsContext(_sharedContext);

        public IDomesticVrpsContext DomesticVrps =>
            new DomesticVrpsContext(_sharedContext);

        public IEntityContext<DomesticPaymentRequest, IDomesticPaymentPublicQuery, DomesticPaymentResponse>
            DomesticPayments =>
            new DomesticPaymentsContext(_sharedContext);
    }
}
