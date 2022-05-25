// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile NatWest { get; }

        private BankProfile GetNatWest()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.NatWest);
            return new BankProfile(
                BankProfileEnum.NatWest,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                "0015800000jfwxXAAQ", // from https://bankofapis.com/articles/consent-confirmation-support/natwest-group-authorisation-servers-explained
                DynamicClientRegistrationApiVersion
                    .Version3p2, // from https://www.bankofapis.com/products/natwest-group-open-banking/dynamic-client-registration/documentation/nwb/1.0
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion
                            .Version3p1p6, // from https://www.bankofapis.com/products/natwest-group-open-banking/payments/documentation/nwb/3.1.6
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiBaseUrl()
                },
                null,
                false)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        ScopeClaimResponseJsonConverter =
                            DelimitedStringConverterOptions.JsonIsStringArrayNotString
                    }
                }
            };
        }
    }
}
