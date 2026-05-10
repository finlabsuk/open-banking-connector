// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class ZopaGenerator : BankProfileGeneratorBase<ZopaBank>
{
    public ZopaGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Zopa) { }

    public override BankProfile GetBankProfile(
        ZopaBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            "https://auth1.openbanking.zopa.com", // from https://developer.openbanking-sandbox.zopa.com/perry/developer/documentation?resource=euhub-zopa-portal&document=docs/30-production.md
            GetFinancialId(bank),
            null,
            null,
            null,
            new PaymentInitiationApi
            {
                ApiVersion = PaymentInitiationApiVersion.Version4p0,
                BaseUrl = GetApiBaseUrl("v4.0/pisp")
            },
            null,
            null,
            true,
            instrumentationClient)
        {
            CustomBehaviour =
                new CustomBehaviourClass
                {
                    BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                    {
                        ClientIdIssuedAtClaimResponseJsonConverter =
                            DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                    },
                    DomesticPaymentConsentAuthGet = new ConsentAuthGetCustomBehaviour
                    {
                        AuthorizationEndpoint =
                            "zopa://open-banking/pis-single-payment-consent" // from https://developer.openbanking-sandbox.zopa.com/perry/developer/documentation?resource=euhub-zopa-portal&document=docs/API%20Overview/pis.md
                    },
                    DomesticPayment =
                        new DomesticPaymentCustomBehaviour
                        {
                            ResponseDataRefundMayBeMissingOrWrong = true,
                            ResponseDataDebtorMayBeMissingOrWrong = true
                        }
                },
            BankConfigurationApiSettings = new BankConfigurationApiSettings { UseRegistrationGetEndpoint = true },
            PispUseV4ByDefault = true
        };

    private static string GetApiBaseUrl(string suffix) =>
        $"https://rs1.openbanking.zopa.com/open-banking/{suffix}"; // from https://developer.openbanking-sandbox.zopa.com/perry/developer/documentation?resource=euhub-zopa-portal&document=docs/30-production.md
}
