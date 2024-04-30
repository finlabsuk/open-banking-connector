﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class SantanderGenerator : BankProfileGeneratorBase<SantanderRegistrationGroup>
{
    public SantanderGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<SantanderRegistrationGroup> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(
        SantanderRegistrationGroup bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroup.GetBankProfile(bank),
            "https://openbanking.santander.co.uk/sanuk/external/open-banking/openid-connect-provider/v1/", // from https://developer.santander.co.uk/sanuk/external/faq-page#t4n553
            GetFinancialId(bank),
            GetAccountAndTransactionApi(bank),
            null,
            null,
            true,
            instrumentationClient)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPost = new BankRegistrationPostCustomBehaviour
                {
                    UseApplicationJoseNotApplicationJwtContentTypeHeader =
                        true
                },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url =
                        "https://openbanking.santander.co.uk/sanuk/external/open-banking/openid-connect-provider/v1/.well-known/openid-configuration" // from https://developer.santander.co.uk/sanuk/external/faq-page#t4n553
                },
                AccountAccessConsentPost =
                    new ConsentPostCustomBehaviour { ResponseLinksOmitId = true }
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationEndpoint = false,
                IdTokenSubClaimType = IdTokenSubClaimType.EndUserId
            },
            AspspBrandId = 15
        };

    private AccountAndTransactionApi? GetAccountAndTransactionApi(SantanderBank bank) =>
        new()
        {
            BaseUrl =
                "https://openbanking-ma.santander.co.uk/sanuk/external/open-banking/v3.1/aisp" // from https://openbanking.atlassian.net/wiki/spaces/AD/pages/110133822/Implementation+Guide+Santander
        };
}
