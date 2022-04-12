// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Danske { get; }

        //See https://developers.danskebank.com/documentation

        private BankProfile GetDanske()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Danske);
            return new BankProfile(
                BankProfileEnum.Danske,
                "https://sandbox-obp-api.danskebank.com/sandbox-open-banking/private", //from https://developers.danskebank.com/documentation#endpoints
                "0015800000jf7AeAAI", //from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=documentation
                DynamicClientRegistrationApiVersion
                    .Version3p2, // from https://developers.danskebank.com/documentation
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersionEnum
                            .Version3p1p6, // from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=documentation
                    BaseUrl =
                        "https://sandbox-obp-api.danskebank.com/sandbox-open-banking/v3.1/pisp" //from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=reference
                },
                null)
            {
                ClientRegistrationApiSettings = new ClientRegistrationApiSettings
                {
                    BankRegistrationAdjustments = (registration, set) =>
                    {
                        registration.UseApplicationJoseNotApplicationJwtContentTypeHeader = true;
                        registration.OpenIdConfigurationOverrides = new OpenIdConfigurationOverrides
                        {
                            //register endpoint response does not provide one
                            ResponseModesSupported = new List<string> { "fragment" }
                        };
                        return registration;
                    },
                }
            };
        }
    }
}
