// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions2
    {
        private BankProfile GetAlliedIrish()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.AlliedIrish);
            return new BankProfile(
                BankProfileEnum.AlliedIrish,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                "0015800000jf9VgAAI", //from https://developer.aibgb.co.uk/sandbox-for-api-testing-gb
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiBaseUrl()
                },
                null,
                false)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                    {
                        ResponseModesSupportedResponse = new List<OAuth2ResponseMode>
                        {
                            // missing from OpenID configuration
                            OAuth2ResponseMode.Fragment
                        }
                    }
                }
            };
        }
    }
}
