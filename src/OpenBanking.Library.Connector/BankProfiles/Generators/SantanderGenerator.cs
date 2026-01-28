// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class SantanderGenerator : BankProfileGeneratorBase<SantanderRegistrationGroup>
{
    public SantanderGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Santander) { }

    public override BankProfile GetBankProfile(
        SantanderRegistrationGroup bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            bank switch
            {
                SantanderBank.Santander =>
                    "https://openbanking.santander.co.uk/sanuk/external/open-banking/openid-connect-provider/v1/", // from https://developer.santander.co.uk/sanuk/external/faq-page#t4n553
                SantanderBank.Personal =>
                    "https://personal-ob.omni.slz.santander.co.uk", // from https://api-portal.omni.slz.santander.co.uk/external/getting-started
                SantanderBank.Business =>
                    "https://business-ob.omni.slz.santander.co.uk", // from https://api-portal.omni.slz.santander.co.uk/external/getting-started
                SantanderBank.Corporate =>
                    "https://corporate-ob.omni.slz.santander.co.uk", // from https://api-portal.omni.slz.santander.co.uk/external/getting-started
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            GetFinancialId(bank),
            bank is SantanderBank.Santander
                ? new AccountAndTransactionApi
                {
                    BaseUrl =
                        "https://openbanking-ma.santander.co.uk/sanuk/external/open-banking/v3.1/aisp" // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110133822/Implementation+Guide+Santander
                }
                : null,
            bank is not SantanderBank.Santander
                ? new AccountAndTransactionApi
                {
                    ApiVersion = AccountAndTransactionApiVersion.Version4p0,
                    BaseUrl = GetV4ApiBaseUrl(bank, "aisp")
                }
                : null,
            bank is SantanderBank.Santander ? new PaymentInitiationApi { BaseUrl = GetV3PaymentsBaseUrl() } : null,
            bank is not SantanderBank.Santander
                ? new PaymentInitiationApi
                {
                    ApiVersion = PaymentInitiationApiVersion.Version4p0,
                    BaseUrl = GetV4ApiBaseUrl(bank, "pisp")
                }
                : null,
            bank is SantanderBank.Santander
                ? new VariableRecurringPaymentsApi { BaseUrl = GetV3PaymentsBaseUrl() }
                : null,
            bank is not SantanderBank.Santander
                ? new VariableRecurringPaymentsApi
                {
                    ApiVersion = VariableRecurringPaymentsApiVersion.Version4p0,
                    BaseUrl = GetV4ApiBaseUrl(bank, "pisp")
                }
                : null,
            true,
            instrumentationClient)
        {
            DynamicClientRegistrationApiVersion =
                bank is not SantanderBank.Santander
                    ? DynamicClientRegistrationApiVersion.Version3p3
                    : DynamicClientRegistrationApiVersion.Version3p2,
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    UseApplicationJoseNotApplicationJwtContentTypeHeader =
                        true
                },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url = bank is not SantanderBank.Santander
                        ? null
                        : "https://openbanking.santander.co.uk/sanuk/external/open-banking/openid-connect-provider/v1/.well-known/openid-configuration" // from https://developer.santander.co.uk/sanuk/external/faq-page#t4n553
                },
                AccountAccessConsentPost =
                    new ReadWritePostCustomBehaviour { PostResponseLinksMayOmitId = true },
                DomesticPayment =
                    new DomesticPaymentCustomBehaviour
                    {
                        ResponseDataDebtorSchemeNameMayBeMissingOrWrong = true,
                        ResponseDataDebtorIdentificationMayBeMissingOrWrong = true
                    },
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                        {
                            IdTokenMayNotHaveNonceClaim = bank is not SantanderBank.Santander,
                            IdTokenMayNotHaveAuthTimeClaim = bank is not SantanderBank.Santander
                        }
                    },
                DomesticVrpConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                        {
                            IdTokenMayNotHaveNonceClaim = bank is not SantanderBank.Santander,
                            IdTokenMayNotHaveAuthTimeClaim = bank is not SantanderBank.Santander
                        }
                    }
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationEndpoint = bank is not SantanderBank.Santander,
                TokenEndpointAuthMethod =
                    bank is not SantanderBank.Santander
                        ? TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt
                        : TokenEndpointAuthMethodSupportedValues.TlsClientAuth,
                IdTokenSubClaimType = IdTokenSubClaimType.EndUserId,
                UseRegistrationGetEndpoint = bank is not SantanderBank.Santander,
                UseRegistrationDeleteEndpoint = bank is not SantanderBank.Santander
            },
            VariableRecurringPaymentsApiSettings = new VariableRecurringPaymentsApiSettings
            {
                DomesticVrpConsentExternalApiRequestAdjustments = request =>
                {
                    request.Data.ControlParameters.PSUInteractionTypes = null;
                    return request;
                },
                DomesticVrpExternalApiRequestAdjustments = request =>
                {
                    request.Data.PSUInteractionType = null;
                    return request;
                },
                UseDomesticVrpConsentPutEndpoint = true
            },
            AispUseV4ByDefault = bank is not SantanderBank.Santander,
            PispUseV4ByDefault = bank is not SantanderBank.Santander,
            VrpUseV4ByDefault = bank is not SantanderBank.Santander,
            AspspBrandId = 15
        };

    private static string GetV4ApiBaseUrl(SantanderBank bank, string suffix) => bank switch
    {
        // from https://api-portal.omni.slz.santander.co.uk/external/documentation/open-banking-account-information-account-info-openapi_4_0,
        // https://api-portal.omni.slz.santander.co.uk/external/documentation/ob-pis-payment-initiation-4_0_0
        SantanderBank.Personal =>
            $"https://personal-api-ob.omni.slz.santander.co.uk/open-banking/v4.0/{suffix}",
        SantanderBank.Business =>
            $"https://business-api-ob.omni.slz.santander.co.uk/open-banking/v4.0/{suffix}",
        SantanderBank.Corporate =>
            $"https://corporate-api-ob.omni.slz.santander.co.uk/open-banking/v4.0/{suffix}",
        _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
    };

    private static string GetV3PaymentsBaseUrl() =>
        "https://openbanking-ma.santander.co.uk/sanuk/external/open-banking/v3.1/pisp"; // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110133822/Implementation+Guide+Santander
}
