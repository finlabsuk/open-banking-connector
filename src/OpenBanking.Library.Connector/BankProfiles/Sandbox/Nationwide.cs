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
        public BankProfile Nationwide { get; }

        private BankProfile GetNationwide()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.Nationwide);
            return new BankProfile(
                BankProfileEnum.Nationwide,
                "https://apionline.obtpp.nationwideinterfaces.io/open-banking", // from https://developer.nationwide.co.uk/open-banking/faqs
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                ClientRegistrationApiVersion.Version3p3, // from https://developer.nationwide.co.uk/dcr-33-tech-implementation-guidance
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion =
                        PaymentInitiationApiVersion
                            .Version3p1p6, // from https://developer.nationwide.co.uk/open-banking/payment-initiation-apis
                    BaseUrl =
                        "https://api.obtpp.nationwideinterfaces.io/open-banking/v3.1/pisp" //from https://developer.nationwide.co.uk/open-banking/payment-initiation-apis#operation/CreateDomesticPaymentConsents
                },
                null)
            {
                ClientRegistrationApiSettings = new ClientRegistrationApiSettings
                {
                    BankRegistrationAdjustments = (registration, set) =>
                    {
                        registration.UseApplicationJoseNotApplicationJwtContentTypeHeader = true;
                        registration.UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues = true;
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
