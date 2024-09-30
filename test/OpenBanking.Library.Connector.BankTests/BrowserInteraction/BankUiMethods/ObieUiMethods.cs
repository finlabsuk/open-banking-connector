// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class ObieUiMethods : IBankUiMethods
{
    private readonly ObieBank _obieBank;

    public ObieUiMethods(BankProfileEnum bankProfileEnum)
    {
        _obieBank = BankGroup.Obie.GetBankGroupData<ObieBank>()
            .GetBank(bankProfileEnum);
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        await page.GetByPlaceholder("username").FillAsync(bankUser.UserNameOrNumber);
        await page.GetByPlaceholder("**********").FillAsync(bankUser.Password);


        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" }).ClickAsync();

        if (consentVariety is ConsentVariety.AccountAccessConsent)
        {
            await page.GetByText("10000109010103").ClickAsync();
            await page.GetByText("Foreign Currency Account").ClickAsync();
        }

        await page.GetByText("10000109010102").ClickAsync();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Confirm" }).ClickAsync();
    }
}
