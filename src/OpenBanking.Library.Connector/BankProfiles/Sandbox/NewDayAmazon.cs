// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile NewDayAmazon { get; }

        private BankProfile GetNewDayAmazon()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.NewDayAmazon);
            return new BankProfile(
                BankProfileEnum.NewDayAmazon,
                "https://api.newdaycards.com/sandbox/identity/v1.0/amazon/", //from https://developer.newdaycards.com/docs/services/AuthenticationSandbox/operations/wellknown
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl =
                        "https://api.newdaycards.com/sandbox/open-banking/v3.1/pisp" // from https://developer.newdaycards.com/docs/services/SandboxOpenBankingPISPAPI/operations/PostDomesticPaymentConsents
                },
                null)
            {
                ClientRegistrationApiSettings = new ClientRegistrationApiSettings
                {
                    BankRegistrationAdjustments = (registration, set) =>
                    {
                        (registration.CustomBehaviour ??= new CustomBehaviour())
                            .BankRegistrationResponseJsonOptions = new BankRegistrationResponseJsonOptions
                            {
                                ScopeConverterOptions = DelimitedStringConverterOptions.JsonIsStringArrayNotString
                            };
                        return registration;
                    },
                }
            };
        }
    }
}
