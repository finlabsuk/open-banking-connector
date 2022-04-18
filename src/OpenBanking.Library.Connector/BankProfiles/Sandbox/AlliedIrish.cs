// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile AlliedIrish { get; }

        private BankProfile GetAlliedIrish()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.AlliedIrish);
            return new BankProfile(
                BankProfileEnum.AlliedIrish,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                "0015800000jf9VgAAI", //from https://developer.aibgb.co.uk/sandbox-for-api-testing-gb
                DynamicClientRegistrationApiVersion
                    .Version3p2, //from https://developer.aibgb.co.uk/dynamic-client-registration-api-v3-2-gb/apis
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
                        registration.CustomBehaviour = new CustomBehaviour{OpenIdConfigurationOverrides = new OpenIdConfigurationOverrides
                        {
                            //well-known endpoint response does not provide one
                            ResponseModesSupported = new List<string> { "fragment" }
                        }};
                        return registration;
                    },
                }
            };
        }
    }
}
