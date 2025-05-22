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

public class HsbcGenerator : BankProfileGeneratorBase<HsbcBank>
{
    public HsbcGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Hsbc) { }

    public override BankProfile GetBankProfile(
        HsbcBank bank,
        IInstrumentationClient instrumentationClient)
    {
        string issuerUrl = bank switch
        {
            HsbcBank.FirstDirect =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/915047304/Implementation+Guide+first+direct
                "https://api.ob.firstdirect.com",
            HsbcBank.Sandbox =>
                GetIssuerUrl(bank),
            HsbcBank.UkBusiness =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1059489023/Implementation+Guide+HSBC+Business
                "https://api.ob.business.hsbc.co.uk",
            HsbcBank.UkKinetic =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1387201093/Implementation+Guide+HSBC+-+Kinetic
                "https://api.ob.hsbckinetic.co.uk",
            HsbcBank.UkPersonal =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/108266712/Implementation+Guide+HSBC+Personal
                "https://api.ob.hsbc.co.uk",
            HsbcBank.HsbcNetUk =>
                // from: https://develop.hsbc.com/sites/default/files/open_banking/HSBC%20Open%20Banking%20TPP%20Implementation%20Guide%20(v3.1).pdf
                "https://api.ob.hsbcnet.com",
            _ => throw new ArgumentOutOfRangeException()
        };
        return new BankProfile(
            _bankGroupData.GetBankProfile(bank),
            issuerUrl,
            GetFinancialId(bank),
            GetAccountAndTransactionApi(bank),
            GetAccountAndTransactionV4Api(bank),
            GetPaymentInitiationApi(bank),
            null,
            GetVariableRecurringPaymentsApi(bank),
            null,
            bank is not HsbcBank.Sandbox,
            instrumentationClient)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    TransportCertificateSubjectDnOrgIdEncoding = SubjectDnOrgIdEncoding.DottedDecimalAttributeType,
                    ClientIdIssuedAtClaimResponseJsonConverter =
                        DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat,
                    AudClaim = issuerUrl,
                    UseApplicationJoseNotApplicationJwtContentTypeHeader = true
                },
                BankRegistrationPut = new BankRegistrationPutCustomBehaviour
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
                },
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                DomesticPaymentConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour { ExpectedResponseRefreshTokenMayBeAbsent = true },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour { PreferMisspeltContractPresentIndicator = true },
                DomesticPayment =
                    new DomesticPaymentCustomBehaviour { PreferMisspeltContractPresentIndicator = true },
                DomesticVrpConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour { ExpectedResponseRefreshTokenMayBeAbsent = true },
                DomesticVrpConsent =
                    new DomesticVrpConsentCustomBehaviour { PreferMisspeltContractPresentIndicator = true },
                DomesticVrp = new DomesticVrpCustomBehaviour
                {
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseDataDebtorAccountMayBeMissingOrWrong = true
                }
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings { UseRegistrationGetEndpoint = true },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.Permissions>
                        {
                            AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                            AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU,
                            AccountAndTransactionModelsPublic.Permissions.ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStatementsDetail
                        };
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }


                    return externalApiRequest;
                }
            },
            DefaultResponseMode = bank is HsbcBank.Sandbox or HsbcBank.UkBusiness
                ? OAuth2ResponseMode.Query
                : OAuth2ResponseMode.Fragment,
            AspspBrandId = bank switch
            {
                HsbcBank.FirstDirect => 7,
                HsbcBank.Sandbox => 10005, // sandbox
                HsbcBank.UkBusiness => 17,
                HsbcBank.UkKinetic => 20,
                HsbcBank.UkPersonal => 9,
                HsbcBank.HsbcNetUk => 9,
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            }
        };
    }

    private AccountAndTransactionApi GetAccountAndTransactionApi(HsbcBank bank) => new()
    {
        BaseUrl = bank switch
        {
            HsbcBank.FirstDirect =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/915047304/Implementation+Guide+first+direct
                "https://api.ob.firstdirect.com/obie/open-banking/v3.1/aisp",
            HsbcBank.Sandbox =>
                GetAccountAndTransactionApiBaseUrl(bank),
            HsbcBank.UkBusiness =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1059489023/Implementation+Guide+HSBC+Business
                "https://api.ob.business.hsbc.co.uk/obie/open-banking/v3.1/aisp",
            HsbcBank.UkKinetic =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1387201093/Implementation+Guide+HSBC+-+Kinetic
                "https://api.ob.hsbckinetic.co.uk/obie/open-banking/v3.1/aisp",
            HsbcBank.UkPersonal =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/108266712/Implementation+Guide+HSBC+Personal
                "https://api.ob.hsbc.co.uk/obie/open-banking/v3.1/aisp",
            HsbcBank.HsbcNetUk =>
                // from: https://develop.hsbc.com/sites/default/files/open_banking/HSBC%20Open%20Banking%20TPP%20Implementation%20Guide%20(v3.1).pdf
                "https://api.ob.hsbcnet.com/obie/open-banking/v3.1/aisp",
            _ => throw new ArgumentOutOfRangeException()
        }
    };

    private AccountAndTransactionApi GetAccountAndTransactionV4Api(HsbcBank bank) => new()
    {
        ApiVersion = AccountAndTransactionApiVersion.Version4p0,
        BaseUrl = bank switch
        {
            // from https://develop.hsbc.com/sites/default/files/open_banking/HSBC%20UK%20Open%20Banking%20Implementation%20Guide%20(v4).pdf
            HsbcBank.FirstDirect =>
                "https://api.ob.firstdirect.com/obie/open-banking/v4.0/aisp",
            HsbcBank.Sandbox =>
                GetAccountAndTransactionApiBaseUrl(bank),
            HsbcBank.UkBusiness =>
                "https://api.ob.business.hsbc.co.uk/obie/open-banking/v4.0/aisp",
            HsbcBank.UkKinetic =>
                "https://api.ob.hsbckinetic.co.uk/obie/open-banking/v4.0/aisp",
            HsbcBank.UkPersonal =>
                "https://api.ob.hsbc.co.uk/obie/open-banking/v4.0/aisp",
            HsbcBank.HsbcNetUk =>
                "https://api.ob.hsbcnet.com/obie/open-banking/v4.0/aisp",
            _ => throw new ArgumentOutOfRangeException()
        }
    };

    private PaymentInitiationApi GetPaymentInitiationApi(HsbcBank bank) => new() { BaseUrl = GetPaymentsBaseUrl(bank) };

    private VariableRecurringPaymentsApi GetVariableRecurringPaymentsApi(HsbcBank bank) => new()
    {
        BaseUrl = GetPaymentsBaseUrl(bank)
    };

    private string GetPaymentsBaseUrl(HsbcBank bank)
    {
        return bank switch
        {
            HsbcBank.FirstDirect =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/915047304/Implementation+Guide+first+direct
                "https://api.ob.firstdirect.com/obie/open-banking/v3.1/pisp",
            HsbcBank.Sandbox =>
                GetPaymentInitiationApiBaseUrl(bank),
            HsbcBank.UkBusiness =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1059489023/Implementation+Guide+HSBC+Business
                "https://api.ob.business.hsbc.co.uk/obie/open-banking/v3.1/pisp",
            HsbcBank.UkKinetic =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1387201093/Implementation+Guide+HSBC+-+Kinetic
                "https://api.ob.hsbckinetic.co.uk/obie/open-banking/v3.1/pisp",
            HsbcBank.UkPersonal =>
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/108266712/Implementation+Guide+HSBC+Personal
                "https://api.ob.hsbc.co.uk/obie/open-banking/v3.1/pisp",
            HsbcBank.HsbcNetUk =>
                // from: https://develop.hsbc.com/sites/default/files/open_banking/HSBC%20Open%20Banking%20TPP%20Implementation%20Guide%20(v3.1).pdf
                "https://api.ob.hsbcnet.com/obie/open-banking/v3.1/pisp",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
