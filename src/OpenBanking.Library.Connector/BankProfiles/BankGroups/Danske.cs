// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups
{
    public enum DanskeBank
    {
        Danske
    }

    public class Danske : BankGroupBase<DanskeBank>
    {
        protected override ConcurrentDictionary<BankProfileEnum, DanskeBank> BankProfileToBank { get; } =
            new()
            {
                [BankProfileEnum.Danske] = DanskeBank.Danske
            };

        //See https://developers.danskebank.com/documentation
        public override BankProfile GetBankProfile(
            BankProfileEnum bankProfileEnum,
            HiddenPropertiesDictionary hiddenPropertiesDictionary)
        {
            DanskeBank bank = GetBank(bankProfileEnum);
            return new BankProfile(
                bankProfileEnum,
                "https://sandbox-obp-api.danskebank.com/sandbox-open-banking/private", //from https://developers.danskebank.com/documentation#endpoints
                "0015800000jf7AeAAI", //from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=documentation
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion
                            .Version3p1p6, // from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=documentation
                    BaseUrl =
                        "https://sandbox-obp-api.danskebank.com/sandbox-open-banking/v3.1/pisp" //from https://developers.danskebank.com/api_products/danske_bank_apis/pi?view=reference
                },
                null,
                false)
            {
                CustomBehaviour = new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        UseApplicationJoseNotApplicationJwtContentTypeHeader = true
                    },
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
