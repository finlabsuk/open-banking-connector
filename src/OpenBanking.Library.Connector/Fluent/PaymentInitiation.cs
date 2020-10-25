// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class PaymentInitiation : IPaymentInitiation
    {
        private readonly ISharedContext _sharedContext;

        public PaymentInitiation(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IFluentContextLocalEntity<DomesticPaymentConsent, DomesticPaymentConsentResponse,
            IDomesticPaymentConsentPublicQuery> DomesticPaymentConsents =>
            new FluentContext<Models.Persistent.PaymentInitiation.DomesticPaymentConsent, DomesticPaymentConsent,
                DomesticPaymentConsentResponse, IDomesticPaymentConsentPublicQuery>(_sharedContext);

        public IFluentContextLocalEntity<DomesticPayment, DomesticPaymentResponse, IDomesticPaymentPublicQuery>
            DomesticPayments => new FluentContext<Models.Persistent.PaymentInitiation.DomesticPayment, DomesticPayment,
            DomesticPaymentResponse, IDomesticPaymentPublicQuery>(_sharedContext);

        public IFluentContextPostOnlyEntity<AuthorisationRedirectObject, AuthorisationRedirectObjectResponse>
            AuthorisationRedirectObjects =>
            new FluentContextPostOnlyEntity<Models.PostOnly.AuthorisationRedirectObject, AuthorisationRedirectObject,
                AuthorisationRedirectObjectResponse>(_sharedContext);
    }
}
