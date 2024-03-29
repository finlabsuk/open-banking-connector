﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class HsbcGenerator : BankProfileGeneratorBase<HsbcBank>
{
    public HsbcGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<HsbcBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(HsbcBank bank)
    {
        (string issuerUrl, string accountAndTransactionApiBaseUrl) = bank switch
        {
            HsbcBank.FirstDirect => (
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/915047304/Implementation+Guide+first+direct
                "https://api.ob.firstdirect.com",
                "https://api.ob.firstdirect.com/obie/open-banking/v3.1/aisp"),
            HsbcBank.Sandbox => (
                GetIssuerUrl(bank),
                GetAccountAndTransactionApiBaseUrl(bank)),
            HsbcBank.UkBusiness => (
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1059489023/Implementation+Guide+HSBC+Business
                "https://api.ob.business.hsbc.co.uk",
                "https://api.ob.business.hsbc.co.uk/obie/open-banking/v3.1/aisp"),
            HsbcBank.UkKinetic => (
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/1387201093/Implementation+Guide+HSBC+-+Kinetic
                "https://api.ob.hsbckinetic.co.uk",
                "https://api.ob.hsbckinetic.co.uk/obie/open-banking/v3.1/aisp"),
            HsbcBank.UkPersonal => (
                // from: https://openbanking.atlassian.net/wiki/spaces/AD/pages/108266712/Implementation+Guide+HSBC+Personal
                "https://api.ob.hsbc.co.uk",
                "https://api.ob.hsbc.co.uk/obie/open-banking/v3.1/aisp"),
            HsbcBank.HsbcNetUk => (
                // from: https://develop.hsbc.com/sites/default/files/open_banking/HSBC%20Open%20Banking%20TPP%20Implementation%20Guide%20(v3.1).pdf
                "https://api.ob.hsbcnet.com",
                "https://api.ob.hsbcnet.com/obie/open-banking/v3.1/aisp"),
            _ => throw new ArgumentOutOfRangeException()
        };
        var sandboxGrantPostCustomBehaviour = new GrantPostCustomBehaviour { DoNotValidateIdToken = true };
        return new BankProfile(
            _bankGroup.GetBankProfile(bank),
            issuerUrl,
            GetFinancialId(bank),
            new AccountAndTransactionApi { BaseUrl = accountAndTransactionApiBaseUrl },
            null,
            null,
            bank is not HsbcBank.Sandbox)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                OpenIdConfigurationGet = bank is HsbcBank.Sandbox
                    ? new OpenIdConfigurationGetCustomBehaviour { Url = GetExtra1(bank) }
                    : null,
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    UseTransportCertificateSubjectDnWithDottedDecimalOrgIdAttribute = true,
                    ClientIdIssuedAtClaimResponseJsonConverter =
                        DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat,
                    AudClaim = issuerUrl,
                    UseApplicationJoseNotApplicationJwtContentTypeHeader = true
                },
                BankRegistrationPut = bank is HsbcBank.Sandbox
                    ? null
                    : new BankRegistrationPutCustomBehaviour { CustomTokenScope = "accounts" },
                JwksGet = bank is HsbcBank.Sandbox
                    ? new JwksGetCustomBehaviour { ResponseHasNoRootProperty = true }
                    : null,
                AccountAccessConsentAuthGet = bank is HsbcBank.Sandbox
                    ? new ConsentAuthGetCustomBehaviour { DoNotValidateIdToken = true }
                    : null,
                AuthCodeGrantPost = bank is HsbcBank.Sandbox
                    ? sandboxGrantPostCustomBehaviour
                    : null,
                RefreshTokenGrantPost = bank is HsbcBank.Sandbox
                    ? sandboxGrantPostCustomBehaviour
                    : null
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings { UseRegistrationGetEndpoint = true },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum>
                        {
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadOffers,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadPartyPSU,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadStatementsDetail
                        };
                    foreach (AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    if (bank is HsbcBank.Sandbox)
                    {
                        externalApiRequest.Data.ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(89);
                    }

                    return externalApiRequest;
                }
            },
            DefaultResponseMode = bank is HsbcBank.Sandbox
                ? OAuth2ResponseMode.Query
                : OAuth2ResponseMode.Fragment
        };
    }
}
