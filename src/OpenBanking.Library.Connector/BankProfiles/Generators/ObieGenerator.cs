// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

//See https://openbanking.atlassian.net/wiki/spaces/DZ/pages/313918598/Integrating+a+TPP+with+Ozone+Model+Banks+Using+Postman+on+Directory+Sandbox#3.1-Dynamic-Client-Registration-(TPP)
public class ObieGenerator : BankProfileGeneratorBase<ObieBank>
{
    public ObieGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<ObieBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(
        ObieBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroup.GetBankProfile(bank),
            "https://auth1.obie.uk.ozoneapi.io", // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration,
            "0015800001041RHAAY", // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration
            new AccountAndTransactionApi
            {
                BaseUrl =
                    "https://rs1.obie.uk.ozoneapi.io/open-banking/v3.1/aisp" // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration
            },
            GetPaymentInitiationApi(bank),
            GetVariableRecurringPaymentsApi(bank),
            false,
            instrumentationClient)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    ClientIdIssuedAtClaimResponseJsonConverter =
                        DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                }
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings { UseRegistrationDeleteEndpoint = true },
            AspspBrandId = 10000 // sandbox
        };

    private static PaymentInitiationApi GetPaymentInitiationApi(ObieBank bank) =>
        new() { BaseUrl = GetPaymentInitiationBaseUrl(bank) };

    private static VariableRecurringPaymentsApi GetVariableRecurringPaymentsApi(ObieBank bank) =>
        new() { BaseUrl = GetPaymentInitiationBaseUrl(bank) };

    private static string GetPaymentInitiationBaseUrl(ObieBank bank) =>
        "https://rs1.obie.uk.ozoneapi.io/open-banking/v3.1/pisp"; // from https://github.com/OpenBankingUK/OBL-ModelBank-Integration;
}
