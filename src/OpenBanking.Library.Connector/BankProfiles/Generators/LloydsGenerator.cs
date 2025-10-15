// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

// Note API discovery endpoints for sandbox:
// https://rs.aspsp.sandbox.lloydsbanking.com/open-banking/discovery
// https://matls.rs.aspsp.sandbox.lloydsbanking.com/open-banking/discovery
public class LloydsGenerator : BankProfileGeneratorBase<LloydsBank>
{
    public LloydsGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Lloyds) { }

    public override BankProfile GetBankProfile(
        LloydsBank bank,
        IInstrumentationClient instrumentationClient)
    {
        return new BankProfile(
            _bankGroupData.GetBankProfile(bank),
            bank switch
            {
                LloydsBank.Sandbox => GetIssuerUrl(LloydsBank.Sandbox),
                LloydsBank.LloydsPersonal =>
                    "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                LloydsBank.LloydsBusiness =>
                    "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/business", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                LloydsBank.LloydsCommerical =>
                    "https://authorise-api.lloydsbank.co.uk/prod01/channel/mtls/lyds/commercial", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                LloydsBank.HalifaxPersonal =>
                    "https://authorise-api.halifax-online.co.uk/prod01/channel/mtls/hfx/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                LloydsBank.BankOfScotlandPersonal =>
                    "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                LloydsBank.BankOfScotlandBusiness =>
                    "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/business", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                LloydsBank.BankOfScotlandCommerical =>
                    "https://authorise-api.bankofscotland.co.uk/prod01/channel/mtls/bos/commercial", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                LloydsBank.MbnaPersonal =>
                    "https://authorise-api.mbna.co.uk/prod01/channel/mtls/mbn/personal", // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide.pdf
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            bank switch
            {
                LloydsBank.Sandbox
                    or LloydsBank.LloydsPersonal
                    or LloydsBank.LloydsBusiness
                    or LloydsBank.LloydsCommerical => GetFinancialId(LloydsBank.LloydsPersonal),
                LloydsBank.HalifaxPersonal
                    or LloydsBank.BankOfScotlandPersonal
                    or LloydsBank.BankOfScotlandBusiness
                    or LloydsBank.BankOfScotlandCommerical => GetFinancialId(LloydsBank.BankOfScotlandPersonal),
                LloydsBank.MbnaPersonal => GetFinancialId(LloydsBank.MbnaPersonal),
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            new AccountAndTransactionApi { BaseUrl = GetApiBaseUrl(bank, "v3.1/aisp") },
            new AccountAndTransactionApi
            {
                ApiVersion = AccountAndTransactionApiVersion.Version4p0,
                BaseUrl = GetApiBaseUrl(bank, "v4.0/aisp")
            },
            new PaymentInitiationApi { BaseUrl = GetPaymentsApiBaseUrl(bank, false) },
            new PaymentInitiationApi
            {
                ApiVersion = PaymentInitiationApiVersion.Version4p0,
                BaseUrl = GetPaymentsApiBaseUrl(bank, true)
            },
            new VariableRecurringPaymentsApi { BaseUrl = GetPaymentsApiBaseUrl(bank, false) },
            new VariableRecurringPaymentsApi
            {
                ApiVersion = VariableRecurringPaymentsApiVersion.Version4p0,
                BaseUrl = GetPaymentsApiBaseUrl(bank, true)
            },
            bank is not LloydsBank.Sandbox,
            instrumentationClient)
        {
            DynamicClientRegistrationApiVersion = DynamicClientRegistrationApiVersion
                .Version3p2, // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide_dcr.pdf?v=1631615723
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = bank is LloydsBank.Sandbox
                    ? new BankRegistrationPostCustomBehaviour
                    {
                        TransportCertificateSubjectDnOrgIdEncoding = SubjectDnOrgIdEncoding.DottedDecimalAttributeType,
                        GrantTypesClaim =
                            new List<OBRegistrationProperties1grantTypesItemEnum>
                            {
                                OBRegistrationProperties1grantTypesItemEnum
                                    .ClientCredentials,
                                OBRegistrationProperties1grantTypesItemEnum
                                    .AuthorizationCode
                            },
                        SubjectTypeClaim = "pairwise"
                    }
                    : new BankRegistrationPostCustomBehaviour
                    {
                        GrantTypesClaim =
                            new List<OBRegistrationProperties1grantTypesItemEnum>
                            {
                                OBRegistrationProperties1grantTypesItemEnum
                                    .ClientCredentials,
                                OBRegistrationProperties1grantTypesItemEnum
                                    .AuthorizationCode
                            },
                        UseApplicationJoseNotApplicationJwtContentTypeHeader =
                            true // from https://developer.lloydsbanking.com/sites/developer.lloydsbanking.com/files/support/technical_implementation_guide_dcr.pdf?v=1631615723
                    },
                BankRegistrationPut = bank is LloydsBank.Sandbox
                    ? null
                    : new BankRegistrationPutCustomBehaviour { GetCustomTokenScope = _ => "openid" },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    ResponseModesSupportedResponse = new List<OAuth2ResponseMode>
                    {
                        // missing from OpenID configuration
                        OAuth2ResponseMode.Fragment
                    }
                },
                AccountAccessConsentAuthGet = bank is LloydsBank.Sandbox
                    ? null
                    : new ConsentAuthGetCustomBehaviour { AddRedundantOAuth2NonceRequestParameter = true },
                AccountAccessConsentPost = bank is LloydsBank.Sandbox
                    ? null
                    : new ReadWritePostCustomBehaviour
                    {
                        PostResponseLinksMayOmitId = true,
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                AccountAccessConsentGet =
                    new ReadWriteGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                AccountGet =
                    new ReadWriteGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                BalanceGet =
                    new ReadWriteGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                DirectDebitGet = bank is LloydsBank.Sandbox
                    ? null
                    : new DirectDebitGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank),
                        PreviousPaymentDateTimeJsonConverter =
                            DateTimeOffsetConverterEnum.JsonInvalidStringBecomesNull
                    },
                Party2Get =
                    new ReadWriteGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                PartyGet =
                    new ReadWriteGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                StandingOrderGet =
                    new ReadWriteGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                TransactionGet =
                    new ReadWriteGetCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = AispResponseLinksAllowReplaceFcn(bank)
                    },
                DomesticPaymentConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour { ExpectedResponseRefreshTokenMayBeAbsent = true },
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour
                    {
                        PreferMisspeltContractPresentIndicator = true,
                        GetResponseLinksAllowReplace = PispResponseLinksAllowReplaceFcn(bank)
                    },
                DomesticPayment =
                    new DomesticPaymentCustomBehaviour
                    {
                        PreferMisspeltContractPresentIndicator = true,
                        GetResponseLinksAllowReplace = PispResponseLinksAllowReplaceFcn(bank),
                        ResponseDataStatusMayBeWrong = bank is LloydsBank.Sandbox,
                        ResponseDataDebtorMayBeMissingOrWrong = bank is LloydsBank.Sandbox,
                        ResponseDataDebtorSchemeNameMayBeMissingOrWrong = true,
                        ResponseDataDebtorIdentificationMayBeMissingOrWrong = true,
                        ResponseDataRefundAccountSchemeNameMayBeMissingOrWrong = bank is LloydsBank.Sandbox,
                        ResponseDataRefundAccountIdentificationMayBeMissingOrWrong = bank is LloydsBank.Sandbox
                    },
                DomesticVrp =
                    new DomesticVrpCustomBehaviour
                    {
                        GetResponseLinksAllowReplace = PispResponseLinksAllowReplaceFcn(bank)
                    },
                DomesticVrpConsent = new DomesticVrpConsentCustomBehaviour
                {
                    GetResponseLinksAllowReplace = PispResponseLinksAllowReplaceFcn(bank)
                },
                DomesticVrpConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true }
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationDeleteEndpoint = true,
                UseRegistrationGetEndpoint = true,
                UseRegistrationAccessToken = bank is LloydsBank.Sandbox,
                IdTokenSubClaimType =
                    bank is LloydsBank.Sandbox ? IdTokenSubClaimType.EndUserId : IdTokenSubClaimType.ConsentId
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove = new List<AccountAndTransactionModelsPublic.Permissions>
                    {
                        AccountAndTransactionModelsPublic.Permissions.ReadPAN
                    };
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                }
            },
            AspspBrandId = bank switch
            {
                LloydsBank.Sandbox => 10004, // sandbox
                LloydsBank.LloydsPersonal
                    or LloydsBank.LloydsBusiness
                    or LloydsBank.LloydsCommerical => 10,
                LloydsBank.HalifaxPersonal => 8,
                LloydsBank.BankOfScotlandPersonal
                    or LloydsBank.BankOfScotlandBusiness
                    or LloydsBank.BankOfScotlandCommerical => 4,
                LloydsBank.MbnaPersonal => 18,
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            AispUseV4ByDefault = false,
            PispUseV4ByDefault = true,
            VrpUseV4ByDefault = true
        };
    }

    private GetResponseLinksAllowReplace AispResponseLinksAllowReplaceFcn(LloydsBank bank) =>
        useV4NotV3 => useV4NotV3 switch
        {
            true => bank switch
            {
                LloydsBank.Sandbox => null,
                LloydsBank.LloydsPersonal
                    or LloydsBank.LloydsBusiness
                    or LloydsBank.LloydsCommerical => null,
                LloydsBank.HalifaxPersonal => (GetApiBaseUrl(LloydsBank.HalifaxPersonal, "v4.0/aisp"),
                    GetApiBaseUrl(LloydsBank.LloydsPersonal, "v4.0/aisp")),
                LloydsBank.BankOfScotlandPersonal
                    or LloydsBank.BankOfScotlandBusiness
                    or LloydsBank.BankOfScotlandCommerical => (
                        GetApiBaseUrl(LloydsBank.BankOfScotlandPersonal, "v4.0/aisp"),
                        GetApiBaseUrl(LloydsBank.LloydsPersonal, "v4.0/aisp")),
                LloydsBank.MbnaPersonal => (GetApiBaseUrl(LloydsBank.MbnaPersonal, "v4.0/aisp"),
                    GetApiBaseUrl(LloydsBank.LloydsPersonal, "v4.0/aisp")),
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            false => null
        };

    private GetResponseLinksAllowReplace PispResponseLinksAllowReplaceFcn(LloydsBank bank) =>
        useV4NotV3 => bank switch
        {
            LloydsBank.Sandbox => null,
            LloydsBank.LloydsPersonal
                or LloydsBank.LloydsBusiness
                or LloydsBank.LloydsCommerical => null,
            LloydsBank.HalifaxPersonal => (GetPaymentsApiBaseUrl(LloydsBank.HalifaxPersonal, useV4NotV3),
                GetPaymentsApiBaseUrl(LloydsBank.LloydsPersonal, useV4NotV3)),
            LloydsBank.BankOfScotlandPersonal
                or LloydsBank.BankOfScotlandBusiness
                or LloydsBank.BankOfScotlandCommerical => (
                    GetPaymentsApiBaseUrl(LloydsBank.BankOfScotlandPersonal, useV4NotV3),
                    GetPaymentsApiBaseUrl(LloydsBank.LloydsPersonal, useV4NotV3)),
            LloydsBank.MbnaPersonal => (GetPaymentsApiBaseUrl(LloydsBank.MbnaPersonal, useV4NotV3),
                GetPaymentsApiBaseUrl(LloydsBank.LloydsPersonal, useV4NotV3)),
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        };

    private string GetPaymentsApiBaseUrl(LloydsBank bank, bool useV4NotV3) =>
        useV4NotV3 ? GetApiBaseUrl(bank, "v4.0/pisp") : GetApiBaseUrl(bank, "v3.1/pisp");

    private string GetApiBaseUrl(LloydsBank bank, string suffix) => bank switch
    {
        LloydsBank.Sandbox => GetPaymentInitiationApiBaseUrl(LloydsBank.Sandbox),
        LloydsBank.LloydsPersonal or LloydsBank.LloydsBusiness or LloydsBank.LloydsCommerical =>
            $"https://secure-api.lloydsbank.com/prod01/lbg/lyds/open-banking/{suffix}", // from https://developer.lloydsbanking.com/node/4045,
        // https://developer.lloydsbanking.com/node/4055#post-domestic-payment-consents-3.1.10
        LloydsBank.HalifaxPersonal =>
            $"https://secure-api.halifax.co.uk/prod01/lbg/hfx/open-banking/{suffix}", // from https://developer.lloydsbanking.com/node/4045,
        // https://developer.lloydsbanking.com/node/4055#post-domestic-payment-consents-3.1.10
        LloydsBank.BankOfScotlandPersonal or LloydsBank.BankOfScotlandBusiness
            or LloydsBank.BankOfScotlandCommerical =>
            $"https://secure-api.bankofscotland.co.uk/prod01/lbg/bos/open-banking/{suffix}", // from https://developer.lloydsbanking.com/node/4045,
        // https://developer.lloydsbanking.com/node/4055#post-domestic-payment-consents-3.1.10
        LloydsBank.MbnaPersonal =>
            $"https://secure-api.mbna.co.uk/prod01/lbg/mbn/open-banking/{suffix}", // from https://developer.lloydsbanking.com/node/4045,
        // https://developer.lloydsbanking.com/node/4055#post-domestic-payment-consents-3.1.10
        _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
    };
}
