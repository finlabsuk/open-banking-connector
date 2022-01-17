// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Monzo { get; }

        private BankProfile GetMonzo()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Monzo);
            return new BankProfile(
                BankProfileEnum.Monzo,
                "https://api.s101.nonprod-ffs.io/open-banking/", //from https://docs.monzo.com/#well-known-endpoints
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                ClientRegistrationApiVersion.Version3p2, // from https://docs.monzo.com/#dynamic-client-registration60
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl = "https://openbanking.s101.nonprod-ffs.io/open-banking/v3.1/pisp" //from https://docs.monzo.com/#well-known-endpoints58
                },
                null);
        }
    }
}
