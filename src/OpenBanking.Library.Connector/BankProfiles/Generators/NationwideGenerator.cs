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
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Nationwide) { }

    public override BankProfile GetBankProfile(
        NationwideBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            "https://obonline.nationwide.co.uk/open-banking/", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
            "0015800000jf8aKAAQ", // from https://developer.nationwide.co.uk/open-banking/how-to?page=1
            new AccountAndTransactionApi { BaseUrl = GetAccountAndTransactionBaseUrl("v3.1") },
            new AccountAndTransactionApi
            {
                BaseUrl = GetAccountAndTransactionBaseUrl("v4.0"),
                ApiVersion = AccountAndTransactionApiVersion.Version4p0
            },
            new PaymentInitiationApi
            {
                BaseUrl =
                    GetPaymentsBaseUrl("v3.1")
            },
            new PaymentInitiationApi
            {
                BaseUrl =
                    GetPaymentsBaseUrl("v4.0"),
                ApiVersion = PaymentInitiationApiVersion.Version4p0
            },
            new VariableRecurringPaymentsApi
            {
                BaseUrl =
                    GetPaymentsBaseUrl("v3.1")
            },
            new VariableRecurringPaymentsApi
            {
                BaseUrl =
                    GetPaymentsBaseUrl("v4.0"),
                ApiVersion = VariableRecurringPaymentsApiVersion.Version4p0
            },
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
                AccountAccessConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                AccountAccessConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                AccountAccessConsentRefreshTokenGrantPost = new RefreshTokenGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                    {
                        IdTokenMayNotHaveAuthTimeClaim = true,
                        DoNotValidateIdTokenAcrClaim = true
                    }
                },
                AccountAccessConsentPost =
                    new ReadWritePostCustomBehaviour { PostResponseLinksMayOmitId = true },
                DomesticPaymentConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                    {
                        DoNotValidateIdTokenAcrClaim = true
                    }
                },
                DomesticPaymentConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    ExpectedResponseRefreshTokenMayBeAbsent = true,
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                DomesticPaymentConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                        {
                            IdTokenMayNotHaveAuthTimeClaim = true,
                            DoNotValidateIdTokenAcrClaim = true
                        }
                    },
                DomesticPaymentConsent = new DomesticPaymentConsentCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    PreferMisspeltContractPresentIndicator = true
                },
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseDataDebtorMayBeMissingOrWrong = true
                },
                DomesticVrpConsentAuthGet =
                    new ConsentAuthGetCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                        {
                            DoNotValidateIdTokenAcrClaim = true
                        }
                    },
                DomesticVrpConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { DoNotValidateIdTokenAcrClaim = true }
                },
                DomesticVrpConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                        {
                            IdTokenMayNotHaveAuthTimeClaim = true,
                            DoNotValidateIdTokenAcrClaim = true
                        }
                    },
                DomesticVrpConsent = new DomesticVrpConsentCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    PreferMisspeltContractPresentIndicator = true
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
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
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
            AspspBrandId = 12,
            AispUseV4ByDefault = true,
            PispUseV4ByDefault = true,
            VrpUseV4ByDefault = true
        };

    private static string GetAccountAndTransactionBaseUrl(string version) =>
        $"https://api.nationwide.co.uk/open-banking/{version}/aisp"; // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide


    private static string GetPaymentsBaseUrl(string version) =>
        $"https://api.nationwide.co.uk/open-banking/{version}/pisp"; // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
}
