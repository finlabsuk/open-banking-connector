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
        public BankProfile RoyalBankOfScotland { get; }

        private BankProfile GetRoyalBankOfScotland()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.RoyalBankOfScotland);
            return new BankProfile(
                BankProfileEnum.RoyalBankOfScotland,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
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
                        registration.BankRegistrationResponseJsonOptions = new BankRegistrationResponseJsonOptions
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
