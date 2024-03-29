﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class BarclaysGenerator : BankProfileGeneratorBase<BarclaysBank>
{
    public BarclaysGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<BarclaysBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(BarclaysBank bank)
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
            bank is not BarclaysBank.Sandbox)
        {
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                AllowNullRegistrationEndpoint = true,
                TokenEndpointAuthMethod = TokenEndpointAuthMethod.PrivateKeyJwt,
                IdTokenSubClaimType = IdTokenSubClaimType.EndUserId,
                BankRegistrationGroup = bank is BarclaysBank.Sandbox
                    ? BankRegistrationGroup.Barclays_Sandbox
                    : BankRegistrationGroup.Barclays_Production
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum>
                        {
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadPAN,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadPartyPSU
                        };
                    if (bank is not BarclaysBank.Barclaycard)
                    {
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadOffers);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStatementsBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStatementsDetail);
                    }

                    if (bank is BarclaysBank.Barclaycard or BarclaysBank.BarclaycardCommercialPayments
                        or BarclaysBank.Corporate)
                    {
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadParty);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadBeneficiariesBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadBeneficiariesDetail);
                    }

                    if (bank is BarclaysBank.Barclaycard or BarclaysBank.BarclaycardCommercialPayments)
                    {
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadStandingOrdersBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadStandingOrdersDetail);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadScheduledPaymentsBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadScheduledPaymentsDetail);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadDirectDebits);
                    }

                    if (bank is BarclaysBank.Sandbox)
                    {
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadParty);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadDirectDebits);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadStandingOrdersBasic);
                        elementsToRemove.Add(
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadStandingOrdersDetail);
                    }

                    foreach (AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum element in
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
                    ? new ConsentAuthGetCustomBehaviour { DoNotValidateIdToken = true }
                    : null,
                AuthCodeGrantPost = bank is BarclaysBank.Sandbox
                    ? new GrantPostCustomBehaviour { DoNotValidateIdToken = true }
                    : null,
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    UseTransportCertificateSubjectDnWithDottedDecimalOrgIdAttribute = true
                }
            }
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
            ApiVersion =
                PaymentInitiationApiVersion
                    .Version3p1p6, // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/998342986/Barclays+Bank+UK+Plc
            BaseUrl = bank is BarclaysBank.Sandbox
                ? "https://sandbox.api.barclays:443/open-banking/v3.1/sandbox/pisp" // from https://developer.barclays.com/apis/payment-initiation/1f6ad5c5-e397-41c0-8d3b-c35446491402.bdn/documentation#interface-details
                : "https://telesto.api.barclays:443/open-banking/v3.1/pisp" // from https://developer.barclays.com/apis/payment-initiation/1f6ad5c5-e397-41c0-8d3b-c35446491402.bdn/documentation#interface-details
        };
}
