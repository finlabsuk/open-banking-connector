// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IPaymentInitiationContext
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        IPostContext<AuthorisationRedirectObject, AuthorisationRedirectObjectResponse>
            AuthorisationRedirectObjects { get; }

        /// <summary>
        ///     API for DomesticPaymentConsent object which corresponds to user consent for a domestic payment.
        /// </summary>
        IApiObjectContext<DomesticPaymentConsent, DomesticPaymentConsentPostResponse, DomesticPaymentConsentGetResponse,
            IDomesticPaymentConsentPublicQuery, DomesticPaymentConsentGetLocalResponse> DomesticPaymentConsents { get; }

        /// <summary>
        ///     API for DomesticPayment which corresponds to a domestic payment.
        /// </summary>
        ILocalEntityContext<DomesticPayment, DomesticPaymentPostResponse, IDomesticPaymentPublicQuery,
                DomesticPaymentGetLocalResponse>
            DomesticPayments { get; }
    }

    internal class PaymentInitiationContext : IPaymentInitiationContext
    {
        private readonly ISharedContext _sharedContext;

        public PaymentInitiationContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IPostContext<AuthorisationRedirectObject, AuthorisationRedirectObjectResponse>
            AuthorisationRedirectObjects =>
            new PostContext<Models.PostOnly.AuthorisationRedirectObject, AuthorisationRedirectObject,
                AuthorisationRedirectObjectResponse>(_sharedContext);

        public IApiObjectContext<DomesticPaymentConsent, DomesticPaymentConsentPostResponse, DomesticPaymentConsentGetResponse,
            IDomesticPaymentConsentPublicQuery, DomesticPaymentConsentGetLocalResponse> DomesticPaymentConsents =>
            new ApiObjectContext<Models.Persistent.PaymentInitiation.DomesticPaymentConsent, DomesticPaymentConsent,
                DomesticPaymentConsentPostResponse, DomesticPaymentConsentGetResponse, IDomesticPaymentConsentPublicQuery,
                DomesticPaymentConsentGetLocalResponse>(_sharedContext);

        public ILocalEntityContext<DomesticPayment, DomesticPaymentPostResponse, IDomesticPaymentPublicQuery,
                DomesticPaymentGetLocalResponse>
            DomesticPayments =>
            new LocalEntityContext<Models.Persistent.PaymentInitiation.DomesticPayment, DomesticPayment,
                DomesticPaymentPostResponse, IDomesticPaymentPublicQuery, DomesticPaymentGetLocalResponse>(_sharedContext);
    }
}
