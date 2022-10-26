// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankGroups;

public class HsbcUiMethods : IBankGroupUiMethods
{
    private readonly Hsbc _bankGroup;

    public HsbcUiMethods(Hsbc bankGroup)
    {
        _bankGroup = bankGroup;
    }

    public async Task PerformConsentAuthUiInteractions(
        BankProfileEnum bankProfileEnum,
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        HsbcBank bank = _bankGroup.GetBank(bankProfileEnum);

        if (bank is HsbcBank.Sandbox)
        {
            // Click input[name="username"]
            await page.Locator("input[name=\"username\"]").ClickAsync();

            // Fill input[name="username"]
            await page.Locator("input[name=\"username\"]").FillAsync(bankUser.UserNameOrNumber);

            // Click input[name="otp"]
            await page.Locator("input[name=\"otp\"]").ClickAsync();

            // Fill input[name="otp"]
            await page.Locator("input[name=\"otp\"]").FillAsync(bankUser.Password);

            // Click button:has-text("Continue")
            await page.RunAndWaitForNavigationAsync(
                async () => { await page.Locator("button:has-text(\"Continue\")").ClickAsync(); });

            // Click .mat-checkbox-inner-container >> nth=0
            await page.WaitForTimeoutAsync(
                400); // workaround for click not registering and then no navigation occurring when "finish" clicked
            await page.Locator(".mat-checkbox-inner-container").First.ClickAsync();
            await page.Locator(".mat-checkbox-inner-container").Nth(1).ClickAsync();

            // Click button:has-text("Finish")
            // await page.WaitForTimeoutAsync(400);
            // await page.ScreenshotAsync(
            //     new PageScreenshotOptions
            //     {
            //         Path = "screenshot.png",
            //         FullPage = true
            //     });
            await page.RunAndWaitForNavigationAsync(
                async () => { await page.Locator("button:has-text(\"Finish\")").ClickAsync(); });
        }
    }

    public bool RequiresManualInteraction(BankProfileEnum bankProfileEnum, ConsentVariety consentVariety)
    {
        HsbcBank bank = _bankGroup.GetBank(bankProfileEnum);
        return bank is not HsbcBank.Sandbox;
    }
}
