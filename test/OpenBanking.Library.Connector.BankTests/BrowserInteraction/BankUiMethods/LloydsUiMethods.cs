// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class LloydsUiMethods : IBankUiMethods
{
    private readonly LloydsBank _lloydsBank;

    public LloydsUiMethods(LloydsBank lloydsBank)
    {
        _lloydsBank = lloydsBank;
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        if (_lloydsBank is LloydsBank.Sandbox)
        {
            // User name
            await page.GetByTestId("fr-field-callback_1").GetByTestId("input-").FillAsync(bankUser.UserNameOrNumber);

            // Password
            await page.GetByTestId("fr-field-callback_2").GetByTestId("input-").FillAsync(bankUser.Password);

            // Next
            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Next" }).ClickAsync();

            // Select account
            await page.Locator("#mat-radio-2 label").ClickAsync();

            // Proceed
            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Proceed" }).ClickAsync();

            // Confirm
            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Yes" }).ClickAsync();
        }
    }
}
