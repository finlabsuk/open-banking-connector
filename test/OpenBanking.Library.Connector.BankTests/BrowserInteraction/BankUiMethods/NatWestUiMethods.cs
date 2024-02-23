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
            await page.Locator("#customer-number").FillAsync(bankUser.UserNameOrNumber);
            await page.Locator("#customer-number").ClickAsync(); // seems necessary
            await page.Locator("#customer-number-login").ClickAsync();

            // Enter credentials
            char[] code = bankUser.Password.ToCharArray();
            foreach (char index in code)
            {
                await page.GetByLabel($"{index}", new PageGetByLabelOptions { Exact = true }).FillAsync($"{index}");
            }
            await page.GetByLabel($"{code[0]}").ClickAsync(); // seems necessary
            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Continue" }).ClickAsync();

            if (consentVariety is ConsentVariety.DomesticPaymentConsent or ConsentVariety.DomesticVrpConsent)
            {
                // Select accounts then confirm access
                switch (_natWestBank)
                {
                    case NatWestBank.NatWestSandbox:
                        await page.Locator("dl")
                            .Filter(
                                new LocatorFilterOptions { HasText = "Personal Savings" })
                            .GetByRole(AriaRole.Button)
                            .ClickAsync();
                        break;
                    case NatWestBank.RoyalBankOfScotlandSandbox:
                        await page.Locator("li")
                            .Filter(new LocatorFilterOptions { HasText = "NameNatWest" })
                            .ClickAsync();
                        break;
                }

                string buttonName = consentVariety switch
                {
                    ConsentVariety.DomesticPaymentConsent => "Confirm payment",
                    ConsentVariety.DomesticVrpConsent => "Confirm VRP",
                    _ => throw new ArgumentOutOfRangeException(nameof(consentVariety), consentVariety, null)
                };
                await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = buttonName }).ClickAsync();
            }
        }
    }
}
