// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IPaymentInitiation
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        IFluentContextPostOnlyEntity<AuthorisationRedirectObject, AuthorisationRedirectObjectResponse>
            AuthorisationRedirectObjects { get; }

        /// <summary>
        ///     API for DomesticPaymentConsent object which corresponds to user consent for a domestic payment.
        /// </summary>
        IFluentContextLocalEntity<DomesticPaymentConsent, DomesticPaymentConsentResponse,
            IDomesticPaymentConsentPublicQuery> DomesticPaymentConsents { get; }

        /// <summary>
        ///     API for DomesticPayment which corresponds to a domestic payment.
        /// </summary>
        IFluentContextLocalEntity<DomesticPayment, DomesticPaymentResponse, IDomesticPaymentPublicQuery>
            DomesticPayments { get; }
    }
}
