// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile Nationwide { get; }

        private BankProfile GetNationwide()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Nationwide);
            return new BankProfile(
                BankProfileEnum.Nationwide,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
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
                        registration.UseApplicationJoseNotApplicationJwtContentTypeHeader = true;
                        registration.BankRegistrationResponseJsonOptions = new BankRegistrationResponseJsonOptions
                        {
                            ClientIdIssuedAtConverterOptions =
                                DateTimeOffsetToUnixConverterOptions.JsonUsesMilliSecondsNotSeconds
                        };
                        return registration;
                    }
                }
            };
        }
    }
}
