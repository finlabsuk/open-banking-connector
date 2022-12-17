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
            // Cookie popup
            await page.Locator("text=Allow cookies").ClickAsync();

            // User name
            await page.Locator("[placeholder=\"User Name\"]").ClickAsync();
            await page.Locator("[placeholder=\"User Name\"]").FillAsync(bankUser.UserNameOrNumber);

            // Password
            await page.Locator("[placeholder=\"Password\"]").ClickAsync();
            await page.Locator("[placeholder=\"Password\"]").FillAsync(bankUser.Password);

            // Next
            await page.RunAndWaitForNavigationAsync(
                async () => { await page.Locator("button:has-text(\"NEXT\")").ClickAsync(); });

            // Select account
            await page.Locator(
                    "label:has-text(\"518791******4295 Interim available : £638.20Interim booked : £11.80\")")
                .ClickAsync();

            // Proceed
            await page.Locator("button:has-text(\"Proceed\")").ClickAsync();

            // Confirm
            await page.Locator("button:has-text(\"Yes\")").ClickAsync();
        }
    }
}
