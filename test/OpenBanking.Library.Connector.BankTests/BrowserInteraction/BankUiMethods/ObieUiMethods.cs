// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class ObieUiMethods : IBankUiMethods
{
    private readonly ObieBank _obieBank;

    public ObieUiMethods(ObieBank obieBank)
    {
        _obieBank = obieBank;
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        if (_obieBank is ObieBank.Modelo)
        {
            await page.GetByLabel("* User Name").ClickAsync();

            await page.GetByLabel("* User Name").FillAsync(bankUser.UserNameOrNumber);

            await page.GetByLabel("* Password").ClickAsync();

            await page.GetByLabel("* Password").FillAsync(bankUser.Password);

            await page.GetByRole(
                AriaRole.Link,
                new PageGetByRoleOptions { NameString = "Login" }).ClickAsync();

            if (consentVariety == ConsentVariety.AccountAccessConsent)
            {
                await page.GetByRole(
                        AriaRole.Listbox,
                        new PageGetByRoleOptions { NameString = "* Accounts" })
                    .SelectOptionAsync(new[] { "700004000000000000000003" });

                // Seems necessary for next click to register
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                await page.GetByRole(
                    AriaRole.Link,
                    new PageGetByRoleOptions { NameString = "Confirm" }).ClickAsync();
            }
            else
            {
                await page.GetByRole(
                        AriaRole.Combobox,
                        new PageGetByRoleOptions { NameString = "* Select debtor account" })
                    .SelectOptionAsync(new[] { "700004000000000000000002" });

                // Seems necessary for next click to register
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                await page.GetByRole(
                    AriaRole.Link,
                    new PageGetByRoleOptions { NameString = "Confirm" }).ClickAsync();
            }
        }
    }
}
