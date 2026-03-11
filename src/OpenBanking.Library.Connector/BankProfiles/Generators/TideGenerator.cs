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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class TideGenerator : BankProfileGeneratorBase<TideBank>
{
    public TideGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Tide) { }

    public override BankProfile GetBankProfile(
        TideBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            "https://issuer1.openbanking.api.tide.co", // from https://developers.tide.co/perry/developer/documentation?resource=tide-uk-portal&document=docs/tpp-onboarding/production.md
            GetFinancialId(bank),
            new AccountAndTransactionApi { BaseUrl = GetApiBaseUrl("aisp") },
            null,
            new PaymentInitiationApi { BaseUrl = GetApiBaseUrl("pisp") },
            null,
            null,
            null,
            true,
            instrumentationClient)
        {
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings { UseReauth = false },
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    TransportCertificateSubjectDnOrgIdEncoding =
                        SubjectDnOrgIdEncoding.DottedDecimalAttributeType,
                    ClientIdIssuedAtClaimResponseJsonConverter =
                        DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                },
                DomesticPaymentConsent = new DomesticPaymentConsentCustomBehaviour
                {
                    GetResponseLinksAllowReplace =
                        _ => ("https://rs1.openbanking.api.tide.co:4501", "http://rs1.openbanking.api.tide.co")
                },
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    ResponseDataDebtorMayBeMissingOrWrong = true,
                    ResponseDataRefundMayBeMissingOrWrong = true
                }
            },
            PaymentInitiationApiSettings = new PaymentInitiationApiSettings
            {
                UseReadRefundAccount = false,
                PreferPartyToPartyPaymentContextCode = true,
                UseContractPresentIndicator = false
            },
            AspspBrandId = 0
        };

    private static string GetApiBaseUrl(string suffix) =>
        $"https://rs1.openbanking.api.tide.co:4501/v1.0/open-banking/v3.1/{suffix}"; // from https://developers.tide.co/perry/developer/documentation?resource=tide-uk-portal&document=docs/tpp-onboarding/production.md
}
