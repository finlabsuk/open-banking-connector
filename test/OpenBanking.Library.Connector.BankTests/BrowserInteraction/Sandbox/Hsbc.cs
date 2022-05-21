// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.Sandbox;

public class Hsbc : IBankProfileUiMethods
{
    public async Task ConsentUiInteractions(IPage page, ConsentVariety consentVariety, BankUser bankUser)
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
        await page.WaitForTimeoutAsync(400); // workaround for click not registering and then no navigation occurring when "finish" clicked
        await page.Locator(".mat-checkbox-inner-container").First.ClickAsync();

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
