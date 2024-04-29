// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class BarclaysGenerator : BankProfileGeneratorBase<BarclaysBank>
{
    public BarclaysGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<BarclaysBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(
        BarclaysBank bank,
        IInstrumentationClient instrumentationClient)
    {
        return new BankProfile(
            _bankGroup.GetBankProfile(bank),
            bank switch
            {
                BarclaysBank
                        .Sandbox =>
                    "https://token.sandbox.barclays.com", // from https://developer.barclays.com/apis/account-and-transactions/20e74071-13fb-44eb-b98f-2c89d6251ad8.bdn/documentation#barclays-identity-provider-(idp)-authentication-types
                BarclaysBank.Personal =>
                    "https://oauth.tiaa.barclays.com/BarclaysPersonal", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/998342986/Barclays+Bank+UK+Plc
                BarclaysBank.Wealth =>
                    "https://oauth.tiaa.barclays.com/BarclaysWealth", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/998342986/Barclays+Bank+UK+Plc
                BarclaysBank.Barclaycard =>
                    "https://oauth.tiaa.barclays.com/BarclaycardUK", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/998342986/Barclays+Bank+UK+Plc
                BarclaysBank.Business =>
                    "https://oauth.tiaa.barclays.com/BarclaysBusiness", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/998342986/Barclays+Bank+UK+Plc
                BarclaysBank.Corporate =>
                    "https://oauth.tiaa.barclays.com/BarclaysCorporate", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/998342986/Barclays+Bank+UK+Plc
                BarclaysBank
                        .BarclaycardCommercialPayments =>
                    "https://oauth.tiaa.barclays.com/BCP", // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/998342986/Barclays+Bank+UK+Plc
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            },
            GetFinancialId(bank),
            GetAccountAndTransactionApi(bank),
            null,
            null,
            bank is not BarclaysBank.Sandbox,
            instrumentationClient)
        {
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationEndpoint = false,
                TokenEndpointAuthMethod = TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt,
                IdTokenSubClaimType = IdTokenSubClaimType.EndUserId
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.Permissions>
                        {
                            AccountAndTransactionModelsPublic.Permissions.ReadPAN,
                            AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU
                        };
                    if (bank is not BarclaysBank.Barclaycard)
                    {
                        elementsToRemove.Add(AccountAndTransactionModelsPublic.Permissions.ReadOffers);
                        elementsToRemove.Add(AccountAndTransactionModelsPublic.Permissions.ReadStatementsBasic);
                        elementsToRemove.Add(AccountAndTransactionModelsPublic.Permissions.ReadStatementsDetail);
                    }

                    if (bank is BarclaysBank.Barclaycard
                        or BarclaysBank.BarclaycardCommercialPayments
                        or BarclaysBank.Corporate)
                    {
                        elementsToRemove.Add(AccountAndTransactionModelsPublic.Permissions.ReadParty);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadBeneficiariesBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadBeneficiariesDetail);
                    }

                    if (bank is BarclaysBank.Barclaycard or BarclaysBank.BarclaycardCommercialPayments)
                    {
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStandingOrdersBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStandingOrdersDetail);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadScheduledPaymentsBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadScheduledPaymentsDetail);
                        elementsToRemove.Add(AccountAndTransactionModelsPublic.Permissions.ReadDirectDebits);
                    }

                    if (bank is BarclaysBank.Sandbox)
                    {
                        elementsToRemove.Add(AccountAndTransactionModelsPublic.Permissions.ReadParty);
                        elementsToRemove.Add(AccountAndTransactionModelsPublic.Permissions.ReadDirectDebits);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStandingOrdersBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStandingOrdersDetail);
                    }

                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                },
                UseGetPartyEndpoint = false
            },
            CustomBehaviour = new CustomBehaviourClass
            {
                AccountAccessConsentPost =
                    new AccountAccessConsentPostCustomBehaviour { ResponseLinksOmitId = true },
                AccountAccessConsentAuthGet = bank is BarclaysBank.Sandbox
                    ? new ConsentAuthGetCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour { DoNotValidateIdToken = true }
                    }
                    : null,
                AccountAccessConsentAuthCodeGrantPost = bank is BarclaysBank.Sandbox
                    ? new AuthCodeGrantPostCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour { DoNotValidateIdToken = true }
                    }
                    : null,
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    TransportCertificateSubjectDnOrgIdEncoding = SubjectDnOrgIdEncoding.DottedDecimalAttributeType
                }
            },
            AspspBrandId = bank is BarclaysBank.Sandbox
                ? 10006 // sandbox
                : 5
        };
    }

    private AccountAndTransactionApi GetAccountAndTransactionApi(BarclaysBank bank) =>
        new()
        {
            BaseUrl = bank is BarclaysBank.Sandbox
                ? "https://sandbox.api.barclays:443/open-banking/v3.1/sandbox/aisp" // from https://developer.barclays.com/apis/account-and-transactions/20e74071-13fb-44eb-b98f-2c89d6251ad8.bdn/documentation#interface-details
                : "https://telesto.api.barclays:443/open-banking/v3.1/aisp" // from https://developer.barclays.com/apis/account-and-transactions/20e74071-13fb-44eb-b98f-2c89d6251ad8.bdn/documentation#interface-details
        };

    private PaymentInitiationApi GetPaymentInitiationApi(BarclaysBank bank) =>
        new()
        {
            BaseUrl = bank is BarclaysBank.Sandbox
                ? "https://sandbox.api.barclays:443/open-banking/v3.1/sandbox/pisp" // from https://developer.barclays.com/apis/payment-initiation/1f6ad5c5-e397-41c0-8d3b-c35446491402.bdn/documentation#interface-details
                : "https://telesto.api.barclays:443/open-banking/v3.1/pisp" // from https://developer.barclays.com/apis/payment-initiation/1f6ad5c5-e397-41c0-8d3b-c35446491402.bdn/documentation#interface-details
        };
}
