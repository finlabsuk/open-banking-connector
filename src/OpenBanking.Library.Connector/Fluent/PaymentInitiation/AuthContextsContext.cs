// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public interface IAuthContextsContext
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        public IPostLocalContext<AuthResult, DomesticPaymentConsentAuthContextResponse> AuthResults { get; }
    }

    internal class AuthContextsContext : IAuthContextsContext
    {
        private readonly ISharedContext _sharedContext;

        public AuthContextsContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public IPostLocalContext<AuthResult, DomesticPaymentConsentAuthContextResponse> AuthResults =>
            new AuthContextsAuthResultsContext(_sharedContext);
    }
}
