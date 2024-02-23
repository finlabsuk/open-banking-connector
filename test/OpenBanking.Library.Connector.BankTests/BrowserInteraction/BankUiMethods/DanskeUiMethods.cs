// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class DanskeUiMethods : IBankUiMethods
{
    private readonly DanskeBank _danskeBank;

    public DanskeUiMethods(DanskeBank danskeBank)
    {
        _danskeBank = danskeBank;
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        await page.GetByLabel("User ID").FillAsync(bankUser.UserNameOrNumber);
        await page.GetByLabel("Password").FillAsync(bankUser.Password);
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Continue" }).ClickAsync();

        await page.Locator(".Radio__label").First.ClickAsync();
        await page.GetByTestId("ContinueButton").ClickAsync();

        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Confirm" }).ClickAsync();
        await page.GetByTestId("ContinueButton").ClickAsync();
    }
}
