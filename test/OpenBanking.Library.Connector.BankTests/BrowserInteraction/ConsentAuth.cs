// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;

public enum ConsentVariety
{
    AccountAccessConsent,
    DomesticPaymentConsent,
    DomesticVrpConsent
}

public class ConsentAuth
{
    private readonly BrowserTypeLaunchOptions _launchOptions;

    public ConsentAuth(BrowserTypeLaunchOptions launchOptions)
    {
        _launchOptions = launchOptions;
    }

    public static IBankProfileUiMethods GetUiMethods(BankProfileEnum bankProfileEnum) =>
        bankProfileEnum switch
        {
            BankProfileEnum.Modelo => new Modelo(),
            BankProfileEnum.NatWest => new NatWest(),
            BankProfileEnum.RoyalBankOfScotland => new RoyalBankOfScotland(),
            BankProfileEnum.Hsbc_Sandbox => new Hsbc(),
            BankProfileEnum.Danske => new Danske(),
            BankProfileEnum.Monzo => new Monzo(),
            BankProfileEnum.Lloyds => new Lloyds(),
            _ => throw new ArgumentException(
                $"{nameof(bankProfileEnum)} is not valid ${nameof(BankProfileEnum)} or needs to be added to this switch statement.")
        };

    public async Task AuthoriseAsync(
        string authUrl,
        BankProfileEnum bankProfileEnum,
        ConsentVariety consentVariety,
        BankUser bankUser)
    {
        using IPlaywright? playwright = await Playwright.CreateAsync();
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(_launchOptions);
        IPage page = await browser.NewPageAsync();
        await page.GotoAsync(authUrl);
        await GetUiMethods(bankProfileEnum).ConsentUiInteractions(page, consentVariety, bankUser);
        // Wait for redirect web page
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.WaitForSelectorAsync(
            "auth-fragment-redirect",
            new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Hidden,
            });

        // Wait 5 secs for redirect
        IJSHandle pageStatusJsHandle = await page.WaitForFunctionAsync(
            "window.pageStatus",
            null,
            new PageWaitForFunctionOptions { Timeout = 10000 });

        var pageStatus = await pageStatusJsHandle.JsonValueAsync<string>();
        if (pageStatus is not "POST of fragment succeeded")
        {
            throw new Exception("Redirect page could not capture and pass on parameters in URL fragment");
        }

        // Wait for token to be available
        await Task.Delay(250);
    }
}
