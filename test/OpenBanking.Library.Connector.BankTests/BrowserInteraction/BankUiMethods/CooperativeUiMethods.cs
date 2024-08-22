// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class CooperativeUiMethods(CooperativeBank cooperativeBank) : IBankUiMethods
{
    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        if (cooperativeBank is CooperativeBank.CooperativeSandbox)
        {
            await page.GetByTestId("userName").FillAsync(bankUser.UserNameOrNumber);
            await page.GetByTestId("submit-button").ClickAsync();
            await page.GetByTestId("password").FillAsync(bankUser.Password);

            await page.GetByTestId("firstInput").FillAsync(bankUser.ExtraWord1);
            await page.GetByTestId("secondInput").FillAsync(bankUser.ExtraWord2);
            await page.GetByTestId("login-enter-security-details-button").ClickAsync();

            await page.GetByTestId("verificationCode").FillAsync(bankUser.ExtraWord3);
            await page.GetByTestId("SubmitButton").ClickAsync();

            await page.GetByTestId("checkbox-0").Locator("div").First.ClickAsync();
            await page.GetByTestId("checkbox-1").Locator("div").First.ClickAsync();
            await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Confirm" }).ClickAsync();
        }
    }
}
