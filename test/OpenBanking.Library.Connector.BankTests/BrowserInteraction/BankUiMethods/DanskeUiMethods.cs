// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;

public class DanskeUiMethods : IBankUiMethods
{
    private readonly DanskeBank _danskeBank;

    public DanskeUiMethods(DanskeBank danskeBank)
    {
        _danskeBank = danskeBank;
    }

    public async Task PerformConsentAuthUiInteractions(
        ConsentVariety consentVariety,
        IPage page,
        BankUser bankUser)
    {
        // Enter user ID and password
        await page.FillAsync("#FakeLogonUserID", bankUser.UserNameOrNumber);
        await page.FillAsync("#FakeLogonPassword", bankUser.Password);

        await page.WaitForTimeoutAsync(400); // workaround for clicks not registering sometimes
        await page.ClickAsync("#FakeLogonContinueButton");

        // Seems necessary to ensure next selector found
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Select account
        await page.ClickAsync(
            "#\\31 55173-12471731 > .Account__controls > .Account__controls__control > .RadioButton > .RadioButton__label");

        await page.WaitForTimeoutAsync(400); // workaround for clicks not registering sometimes
        await page.ClickAsync("#confirm");

        // Confirm consent
        await page.WaitForTimeoutAsync(400); // workaround for clicks not registering sometimes
        await page.ClickAsync("#ASHESignatureConfirmButton");

        // Transfer back
        await page.WaitForTimeoutAsync(400); // workaround for clicks not registering sometimes
        await page.ClickAsync("#transfer");
    }
}
