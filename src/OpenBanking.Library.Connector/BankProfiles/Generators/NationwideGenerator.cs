// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class NationwideGenerator : BankProfileGeneratorBase<NationwideBank>
{
    public NationwideGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<NationwideBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(
        NationwideBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroup.GetBankProfile(bank),
            "https://obonline.nationwide.co.uk/open-banking/", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
            "0015800000jf8aKAAQ", // from https://developer.nationwide.co.uk/open-banking/how-to?page=1
            GetAccountAndTransactionApi(bank),
            GetPaymentInitiationApi(bank),
            GetVariableRecurringPaymentsApi(bank),
            true,
            instrumentationClient)
        {
            DynamicClientRegistrationApiVersion = DynamicClientRegistrationApiVersion.Version3p3,
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    TransportCertificateSubjectDnOrgIdEncoding =
                        SubjectDnOrgIdEncoding.DottedDecimalAttributeTypeWithStringValue,
                    UseApplicationJoseNotApplicationJwtContentTypeHeader =
                        true,
                    ClientIdIssuedAtClaimResponseJsonConverter =
                        DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url =
                        "https://obonline.nationwide.co.uk/open-banking/.well-known/openid-configuration" // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
                },
                AccountAccessConsentRefreshTokenGrantPost = new RefreshTokenGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAuthTimeClaim = true }
                },
                AccountAccessConsentPost =
                    new ReadWritePostCustomBehaviour { PostResponseLinksMayOmitId = true },
                DomesticPaymentConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour { ExpectedResponseRefreshTokenMayBeAbsent = true },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour
                    {
                        PostResponseLinksMayOmitId = true,
                        PreferMisspeltContractPresentIndicator = true
                    },
                DomesticPaymentConsentRefreshTokenGrantPost = new RefreshTokenGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAuthTimeClaim = true }
                },
                DomesticPayment =
                    new DomesticPaymentCustomBehaviour
                    {
                        PostResponseLinksMayOmitId = true,
                        PreferMisspeltContractPresentIndicator = true,
                        ResponseDataDebtorMayBeMissingOrWrong = true
                    },
                DomesticVrpConsent =
                    new DomesticVrpConsentCustomBehaviour
                    {
                        PostResponseLinksMayOmitId = true,
                        PreferMisspeltContractPresentIndicator = true
                    },
                DomesticVrpConsentRefreshTokenGrantPost = new RefreshTokenGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAuthTimeClaim = true }
                },
                DomesticVrp = new DomesticVrpCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseLinksMayHaveIncorrectUrlBeforeQuery = true
                }
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove = new List<AccountAndTransactionModelsPublic.Permissions>
                    {
                        AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU
                    };
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                },
                UseGetPartyEndpoint = false
            },
            AspspBrandId = 12
        };

    private AccountAndTransactionApi? GetAccountAndTransactionApi(NationwideBank bank) =>
        new()
        {
            BaseUrl =
                "https://api.nationwide.co.uk/open-banking/v3.1/aisp" // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
        };

    private PaymentInitiationApi? GetPaymentInitiationApi(NationwideBank bank) =>
        new()
        {
            BaseUrl =
                GetPaymentsBaseUrl()
        };

    private VariableRecurringPaymentsApi? GetVariableRecurringPaymentsApi(NationwideBank bank) =>
        new()
        {
            BaseUrl =
                GetPaymentsBaseUrl()
        };


    private static string GetPaymentsBaseUrl() =>
        "https://api.nationwide.co.uk/open-banking/v3.1/pisp"; // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
}
