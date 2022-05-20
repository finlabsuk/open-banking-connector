// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public interface IBankProfileDefinitions
    {
        BankProfile GetBankProfile(BankProfileEnum bankProfileEnum);
    }

    public class BankProfileDefinitionsStub : IBankProfileDefinitions
    {
        public BankProfile GetBankProfile(BankProfileEnum bankProfileEnum)
        {
            throw new NotImplementedException();
        }
    }

    public partial class BankProfileDefinitions: IBankProfileDefinitions
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
                DynamicClientRegistrationApiVersion
                    .Version3p3, // from https://developer.nationwide.co.uk/dcr-33-tech-implementation-guidance
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
                BankConfigurationApiSettings = new BankConfigurationApiSettings
                {
                    BankRegistrationAdjustments = registration =>
                    {
                        BankRegistrationPostCustomBehaviour bankRegistrationPost =
                            (registration.CustomBehaviour ??= new CustomBehaviourClass())
                            .BankRegistrationPost ??= new BankRegistrationPostCustomBehaviour();
                        bankRegistrationPost.ClientIdIssuedAtClaimResponseJsonConverter =
                            DateTimeOffsetConverter.UnixMilliSecondsJsonFormat;
                        bankRegistrationPost.UseApplicationJoseNotApplicationJwtContentTypeHeader = true;
                        bankRegistrationPost
                            .UseTransportCertificateDnWithStringNotHexDottedDecimalAttributeValues = true;

                        return registration;
                    }
                }
            };
        }
    }
}
