// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Hsbc { get; }

        private BankProfile GetHsbc()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Hsbc);
            return new BankProfile(
                BankProfileEnum.Hsbc,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiBaseUrl()
                },
                null)
            {
                ClientRegistrationApiSettings = new ClientRegistrationApiSettings
                {
                    BankRegistrationAdjustments = (registration, scope) =>
                    {
                        registration.CustomBehaviour= new CustomBehaviour{OAuth2RequestObjectClaimsOverrides =
                            new OAuth2RequestObjectClaimsOverrides
                            {
                                Issuer = bankProfileHiddenProperties.GetAdditionalProperty1(),
                                Audience = bankProfileHiddenProperties.GetAdditionalProperty2()
                            }};
                        return registration;
                    },
                },
            };
        }
    }
}
