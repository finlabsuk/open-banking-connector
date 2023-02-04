// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class NatWestUiMethods : IBankUiMethods
{
    private readonly NatWestBank _natWestBank;

    public NatWestUiMethods(NatWestBank natWestBank)
    {
        _natWestBank = natWestBank;
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        if (_natWestBank is NatWestBank.NatWestSandbox or NatWestBank.RoyalBankOfScotlandSandbox)
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

            if (consentVariety is ConsentVariety.DomesticPaymentConsent)
            {
                // Select accounts then confirm access
                string selector = _natWestBank switch
                {
                    NatWestBank.NatWestSandbox =>
                        "#account-list > li:nth-child(2) > dl > dd.action.col-size-1 > button",
                    NatWestBank.RoyalBankOfScotlandSandbox =>
                        "#account-list > .row-element:nth-child(2) > dl > .action > button",
                    _ => throw new ArgumentOutOfRangeException()
                };
                await page.ClickAsync(selector);
                await page.ClickAsync("#approveButton");
            }
        }
    }
}
