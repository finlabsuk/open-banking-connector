// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions2
    {
        private BankProfile GetBarclays()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Barclays);
            return new BankProfile(
                BankProfileEnum.Barclays,
                "https://token.sandbox.barclays.com", //from https://developer.barclays.com/apis/account-and-transactions/20e74071-13fb-44eb-b98f-2c89d6251ad8.bdn/documentation
                bankProfileHiddenProperties.GetRequiredFinancialId(),
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
                //placeholder value since missing from barclays well-known endpoint (openID configuration) 
                //registration.RegistrationEndpoint = "https://example.com/register";
            };
        }
    }
}
