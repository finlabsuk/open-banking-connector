// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Modelo { get; }

        //See https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
        private BankProfile GetModelo()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Modelo);
            return new BankProfile(
                BankProfileEnum.Modelo,
                "https://ob19-auth1-ui.o3bank.co.uk", //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                "0015800001041RHAAY", //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                DynamicClientRegistrationApiVersion
                    .Version3p2, // inferred from registration_endpoint in : https://ob19-auth1-ui.o3bank.co.uk/.well-known/openid-configuration
                new AccountAndTransactionApi
                {
                    AccountAndTransactionApiVersion = AccountAndTransactionApiVersion.Version3p1p9,
                    BaseUrl =
                        "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/aisp" // from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+the+Model+Bank+provided+by+OBIE#Accounts-End-points
                },
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl =
                        "https://ob19-rs1.o3bank.co.uk:4501/open-banking/v3.1/pisp" //from https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
                },
                null)
            {
                BankConfigurationApiSettings = new BankConfigurationApiSettings
                {
                    BankRegistrationAdjustments = registration =>
                    {
                        (registration.CustomBehaviour ??= new CustomBehaviourClass())
                            .BankRegistrationPost =
                            new BankRegistrationPostCustomBehaviour
                            {
                                ClientIdIssuedAtClaimResponseJsonConverter =
                                    DateTimeOffsetConverter.UnixMilliSecondsJsonFormat
                            };
                        return registration;
                    },
                }
            };
        }
    }
}
