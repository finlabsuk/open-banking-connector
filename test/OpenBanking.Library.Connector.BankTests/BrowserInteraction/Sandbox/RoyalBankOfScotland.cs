// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.Sandbox;

public class RoyalBankOfScotland : IBankProfileUiMethods
{
    public async Task ConsentUiInteractions(IPage page, ConsentVariety consentVariety, BankUser bankUser)
    {
        // Enter customer number
        await page.Locator("input[name=\"customer-number\"]").FillAsync(bankUser.UserNameOrNumber);
        await page.Locator("input[name=\"customer-number\"]").ClickAsync(); // seems necessary
        await page.Locator("#customer-number-login").ClickAsync();

        // Complete upper row of boxes
        var elements = new List<string>
        {
            "#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(2) > div.panel-body > div:nth-child(1) > label",
            "#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(2) > div.panel-body > div:nth-child(2) > label",
            "#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(2) > div.panel-body > div:nth-child(3) > label",
            "#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(3) > div.panel-body > div:nth-child(1) > label",
            "#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(3) > div.panel-body > div:nth-child(2) > label",
            "#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(3) > div.panel-body > div:nth-child(3) > label"
        };
        var textBoxes = new List<string>
        {
            "#pin-1",
            "#pin-2",
            "#pin-3",
            "#password-1",
            "#password-2",
            "#password-3"
        };
        for (var idx = 0; idx < elements.Count; idx++)
        {
            IElementHandle handle = (await page.WaitForSelectorAsync(elements[idx]))!;
            var digit = await page.EvaluateAsync<string>("el => el.textContent", handle);
            await page.FillAsync(textBoxes[idx], digit);
        }

        // Click continue
        await page.ClickAsync(textBoxes[elements.Count - 1]); // seems necessary
        await page.ClickAsync("#login-button");

        // Select accounts then confirm access
        await page.ClickAsync("#account-list > .row-element:nth-child(2) > dl > .action > button");
        await page.ClickAsync("#approveButton");
    }
}
