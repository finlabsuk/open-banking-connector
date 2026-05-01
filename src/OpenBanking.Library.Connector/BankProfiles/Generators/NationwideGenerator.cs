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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
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
            bank switch
            {
                NationwideBank.Nationwide =>
                    "https://obonline.nationwide.co.uk/open-banking/", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
                NationwideBank
                        .VirginMerged =>
                    "https://api.openbanking.virginmoney.com", // from https://developer.virginmoney.com/merged/dynamic-registration/
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            "0015800000jf8aKAAQ", // from https://developer.nationwide.co.uk/open-banking/how-to?page=1
            new AccountAndTransactionApi { BaseUrl = GetApiBaseUrl(bank, "v3.1/aisp") },
            bank is NationwideBank.Nationwide
                ? new AccountAndTransactionApi
                {
                    BaseUrl = GetApiBaseUrl(bank, "v4.0/aisp"),
                    ApiVersion = AccountAndTransactionApiVersion.Version4p0
                }
                : null,
            new PaymentInitiationApi { BaseUrl = GetApiBaseUrl(bank, "v3.1/pisp") },
            bank is NationwideBank.Nationwide
                ? new PaymentInitiationApi
                {
                    BaseUrl = GetApiBaseUrl(bank, "v4.0/pisp"),
                    ApiVersion = PaymentInitiationApiVersion.Version4p0
                }
                : null,
            bank is NationwideBank.Nationwide
                ? new VariableRecurringPaymentsApi { BaseUrl = GetApiBaseUrl(bank, "v3.1/pisp") }
                : null,
            bank is NationwideBank.Nationwide
                ? new VariableRecurringPaymentsApi
                {
                    BaseUrl = GetApiBaseUrl(bank, "v4.0/pisp"),
                    ApiVersion = VariableRecurringPaymentsApiVersion.Version4p0
                }
                : null,
            true,
            instrumentationClient)
        {
            DynamicClientRegistrationApiVersion = bank switch
            {
                NationwideBank.Nationwide => DynamicClientRegistrationApiVersion.Version3p3,
                NationwideBank.VirginMerged => DynamicClientRegistrationApiVersion.Version3p2,
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    TransportCertificateSubjectDnOrgIdEncoding =
                        bank is NationwideBank.Nationwide
                            ? SubjectDnOrgIdEncoding.DottedDecimalAttributeTypeWithStringValue
                            : null,
                    UseApplicationJoseNotApplicationJwtContentTypeHeader =
                        true,
                    IssuedAtClaimResponseJsonConverter =
                        bank is NationwideBank.VirginMerged
                            ? DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                            : null,
                    ExpirationTimeClaimResponseJsonConverter =
                        bank is NationwideBank.VirginMerged
                            ? DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                            : null,
                    ClientIdIssuedAtClaimResponseJsonConverter =
                        bank is NationwideBank.Nationwide
                            ? DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                            : null
                },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url = bank switch
                    {
                        NationwideBank.Nationwide =>
                            "https://obonline.nationwide.co.uk/open-banking/.well-known/openid-configuration", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
                        NationwideBank.VirginMerged =>
                            "https://api.openbanking.virginmoney.com/open-banking/v3.0/.well-known/openid-configuration", // from https://developer.virginmoney.com/merged/dynamic-registration/
                        _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
                    }
                },
                AccountAccessConsentAuthGet =
                    new ConsentAuthGetCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                        {
                            IdTokenSubClaimType =
                                bank is NationwideBank.VirginMerged ? IdTokenSubClaimType.ClientId : null,
                            IdTokenMayNotHaveAuthTimeClaim = bank is NationwideBank.VirginMerged,
                            DoNotValidateIdTokenAcrClaim = bank is NationwideBank.Nationwide,
                            IdTokenMayNotHaveAcrClaim = bank is NationwideBank.VirginMerged
                        }
                    },
                AccountAccessConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour
                    {
                        UseScopeInRequest = bank is NationwideBank.VirginMerged,
                        Scope = bank is NationwideBank.VirginMerged ? "accounts" : null,
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour
                            {
                                IdTokenMayNotHaveNonceClaim = bank is NationwideBank.VirginMerged,
                                IdTokenMayNotHaveAuthTimeClaim = bank is NationwideBank.VirginMerged,
                                DoNotValidateIdTokenAcrClaim = bank is NationwideBank.Nationwide
                            }
                    },
                AccountAccessConsentRefreshTokenGrantPost = new RefreshTokenGrantPostCustomBehaviour
                {
                    UseScopeInRequest = bank is NationwideBank.VirginMerged,
                    Scope = bank is NationwideBank.VirginMerged ? "accounts" : null,
                    IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                    {
                        IdTokenMayNotHaveNonceClaim = bank is NationwideBank.VirginMerged,
                        IdTokenMayNotHaveAuthTimeClaim = true,
                        DoNotValidateIdTokenAcrClaim = bank is NationwideBank.Nationwide
                    }
                },
                AccountAccessConsentPost =
                    new ReadWritePostCustomBehaviour { PostResponseLinksMayOmitId = true },
                BankRegistrationPut = bank is NationwideBank.VirginMerged
                    ? new BankRegistrationPutCustomBehaviour
                    {
                        GetCustomTokenScope = registrationScope =>
                        {
                            if ((registrationScope & RegistrationScopeEnum.AccountAndTransaction) ==
                                RegistrationScopeEnum.AccountAndTransaction)
                            {
                                return "accounts";
                            }
                            if ((registrationScope & RegistrationScopeEnum.PaymentInitiation) ==
                                RegistrationScopeEnum.PaymentInitiation)
                            {
                                return "payments";
                            }
                            if ((registrationScope & RegistrationScopeEnum.FundsConfirmation) ==
                                RegistrationScopeEnum.FundsConfirmation)
                            {
                                return "fundsconfirmations";
                            }
                            throw new Exception("Cannot determine custom token scope.");
                        }
                    }
                    : null,
                DomesticPaymentConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour = new IdTokenProcessingCustomBehaviour
                    {
                        IdTokenSubClaimType =
                            bank is NationwideBank.VirginMerged ? IdTokenSubClaimType.ClientId : null,
                        IdTokenMayNotHaveAuthTimeClaim = bank is NationwideBank.VirginMerged,
                        DoNotValidateIdTokenAcrClaim = true
                    }
                },
                DomesticPaymentConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour
                    {
                        ExpectedResponseRefreshTokenMayBeAbsent = true,
                        UseScopeInRequest = bank is NationwideBank.VirginMerged,
                        Scope = bank is NationwideBank.VirginMerged ? "payments" : null,
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour
                            {
                                IdTokenMayNotHaveNonceClaim = bank is NationwideBank.VirginMerged,
                                IdTokenMayNotHaveAuthTimeClaim = bank is NationwideBank.VirginMerged,
                                DoNotValidateIdTokenAcrClaim = true
                            }
                    },
                DomesticPaymentConsent = new DomesticPaymentConsentCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    PreferMisspeltContractPresentIndicator = true,
                    UseB64JoseHeader = bank is NationwideBank.VirginMerged
                },
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseDataDebtorMayBeMissingOrWrong = true,
                    ResponseDataRefundMayBeMissingOrWrong = bank is NationwideBank.VirginMerged,
                    UseB64JoseHeader = bank is NationwideBank.VirginMerged
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
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseLinksMayHaveIncorrectUrlBeforeQuery = true
                },
                DomesticVrp = new DomesticVrpCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseLinksMayHaveIncorrectUrlBeforeQuery = true
                }
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationDeleteEndpoint = bank is NationwideBank.VirginMerged,
                UseRegistrationGetEndpoint = bank is NationwideBank.VirginMerged,
                TokenEndpointAuthMethod = bank switch
                {
                    NationwideBank.Nationwide => TokenEndpointAuthMethodSupportedValues.TlsClientAuth,
                    NationwideBank.VirginMerged => TokenEndpointAuthMethodSupportedValues.ClientSecretBasic,
                    _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
                }
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    List<AccountAndTransactionModelsPublic.Permissions> elementsToRemove =
                            bank is NationwideBank.VirginMerged
                                ? new List<AccountAndTransactionModelsPublic.Permissions>
                                {
                                    AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                                    AccountAndTransactionModelsPublic.Permissions.ReadProducts
                                }
                                : new List<AccountAndTransactionModelsPublic.Permissions>
                                {
                                    AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU
                                }
                        ;
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                },
                AccountAccessConsentPostCreateDelaySeconds = bank is NationwideBank.VirginMerged ? 30 : 0,
                UseGetParty2Endpoint = bank is NationwideBank.Nationwide,
                UseGetPartyEndpoint = bank is NationwideBank.VirginMerged
            },
            PaymentInitiationApiSettings = new PaymentInitiationApiSettings
            {
                PreferPartyToPartyPaymentContextCode = bank is NationwideBank.VirginMerged,
                UseContractPresentIndicator = bank is NationwideBank.Nationwide,
                UseReadRefundAccount = bank is NationwideBank.Nationwide
            },
            DefaultResponseMode =
                bank is NationwideBank.VirginMerged ? OAuth2ResponseMode.Query : OAuth2ResponseMode.Fragment,
            AispUseV4ByDefault = bank is NationwideBank.Nationwide,
            PispUseV4ByDefault = bank is NationwideBank.Nationwide,
            VrpUseV4ByDefault = bank is NationwideBank.Nationwide
        };

    private static string GetApiBaseUrl(NationwideBank bank, string suffix) =>
        bank switch
        {
            NationwideBank.Nationwide =>
                $"https://api.nationwide.co.uk/open-banking/{suffix}", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110101211/Implementation+Guide+Nationwide
            NationwideBank.VirginMerged =>
                $"https://api.openbanking.virginmoney.com/open-banking/{suffix}", // from https://developer.virginmoney.com/merged/account-access-consents/,
            // https://developer.virginmoney.com/merged/dip/
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        };
}
