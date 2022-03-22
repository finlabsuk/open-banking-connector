// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Barclays { get; }

        private BankProfile GetBarclays()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Barclays);
            return new BankProfile(
                BankProfileEnum.Barclays,
                "https://token.sandbox.barclays.com", //from https://developer.barclays.com/apis/account-and-transactions/20e74071-13fb-44eb-b98f-2c89d6251ad8.bdn/documentation
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
                    BankRegistrationAdjustments = (registration, set) =>
                    {
                        registration.OpenIdConfigurationOverrides = new OpenIdConfigurationOverrides
                        {
                            //placeholder value since missing from barclays well-known endpoint (openID configuration) 
                            RegistrationEndpoint = "https://example.com/register"
                        };
                        return registration;
                    },
                }
            };
        }
    }
}
