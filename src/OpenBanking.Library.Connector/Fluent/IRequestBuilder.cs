// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IRequestBuilder
    {
        IFluentContextLocalEntity<Bank, BankResponse, IBankPublicQuery> Banks { get; }

        IFluentContextLocalEntity<BankProfile, BankProfileResponse, IBankProfilePublicQuery> BankProfiles { get; }

        IFluentContextLocalEntity<BankRegistration, BankRegistrationResponse, IBankRegistrationPublicQuery>
            BankRegistrations { get; }

        IFluentContextPostOnlyEntity<AuthorisationRedirectObject, AuthorisationRedirectObjectResponse>
            AuthorisationRedirectObjects { get; }

        IFluentContextLocalEntity<DomesticPaymentConsent, DomesticPaymentConsentResponse,
            IDomesticPaymentConsentPublicQuery> DomesticPaymentConsents { get; }

        IFluentContextLocalEntity<DomesticPayment, DomesticPaymentResponse, IDomesticPaymentPublicQuery>
            DomesticPayments { get; }

        SoftwareStatementProfileContext SoftwareStatementProfile();
    }
}
