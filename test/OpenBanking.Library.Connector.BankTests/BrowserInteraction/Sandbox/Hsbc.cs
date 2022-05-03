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
        await page.ClickAsync("#username");
        await page.FillAsync("#username", bankUser.UserNameOrNumber);

        await page.ClickAsync("#otp");
        await page.FillAsync("#otp", bankUser.Password);

        await page.ClickAsync(".form-inner > .submit-btn > .btn-container > .continuebtn > a");
    }
}
