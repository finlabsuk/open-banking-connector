// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class ObieUiMethods : IBankUiMethods
{
    private readonly ObieBank _obieBank;

    public ObieUiMethods(ObieBank obieBank)
    {
        _obieBank = obieBank;
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        await page.ClickAsync("#wizardContent > #loginForm #loginName");
        await page.FillAsync("#wizardContent > #loginForm #loginName", bankUser.UserNameOrNumber);

        await page.ClickAsync("#wizardContent > #loginForm #password");
        await page.FillAsync("#wizardContent > #loginForm #password", bankUser.Password);

        await page.ClickAsync(".col-md-9 > #wizardContent > .nav > .nav-item:nth-child(1) > .nav-link");

        if (consentVariety == ConsentVariety.AccountAccessConsent)
        {
            await page.SelectOptionAsync("#selectAccountsPage > #loginForm #accounts", "700004000000000000000002");
            await page.ClickAsync("#loginForm > .form-row > .form-group > #accounts > option:nth-child(2)");
        }
        else
        {
            await page.ClickAsync("#selectAccountsPage > #loginForm #account");
        }

        await page.ClickAsync("#wizardContent > #selectAccountsPage > .nav > .nav-item:nth-child(1) > .nav-link");
    }
}
