// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class BarclaysUiMethods : IBankUiMethods
{
    private readonly BarclaysBank _barclaysBank;

    public BarclaysUiMethods(BankProfileEnum bankProfileEnum)
    {
        _barclaysBank = BankGroup.Barclays.GetBankGroupData<BarclaysBank>()
            .GetBank(bankProfileEnum);
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        if (_barclaysBank is BarclaysBank.Sandbox)
        {
            await page.Locator("#api-type").SelectOptionAsync(new[] { "Account And Transactions" });

            await page.Locator("#business-unit").SelectOptionAsync(new[] { "Barclays Personal" });

            await page.Locator("#test-case").SelectOptionAsync(new[] { "ACC200" });

            await page.GetByRole(
                    AriaRole.Button,
                    new PageGetByRoleOptions { NameString = "Run Test Case" })
                .ClickAsync();
        }
    }
}
