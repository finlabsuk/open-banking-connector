// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankGroups;
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
    private readonly IBankProfileDefinitions _bankProfileDefinitions;
    private readonly BrowserTypeLaunchOptions _launchOptions;


    public ConsentAuth(BrowserTypeLaunchOptions launchOptions, IBankProfileDefinitions bankProfileDefinitions)
    {
        _launchOptions = launchOptions;
        _bankProfileDefinitions = bankProfileDefinitions;
    }

    private Task ConsentUiInteractions(
        BankProfileEnum bankProfileEnum,
        IPage page,
        ConsentVariety consentVariety,
        BankUser bankUser)
    {
        return _bankProfileDefinitions.GetBankGroup(_bankProfileDefinitions.GetBankGroupEnum(bankProfileEnum)) switch
        {
            Danske danske => danske.ConsentUiInteractions(bankProfileEnum, page, consentVariety, bankUser),
            Hsbc hsbc => hsbc.ConsentUiInteractions(bankProfileEnum, page, consentVariety, bankUser),
            Lloyds lloyds => lloyds.ConsentUiInteractions(bankProfileEnum, page, consentVariety, bankUser),
            Monzo monzo => monzo.ConsentUiInteractions(bankProfileEnum, page, consentVariety, bankUser),
            NatWest natWest => natWest.ConsentUiInteractions(bankProfileEnum, page, consentVariety, bankUser),
            Obie obie => obie.ConsentUiInteractions(bankProfileEnum, page, consentVariety, bankUser),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
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
        await ConsentUiInteractions(bankProfileEnum, page, consentVariety, bankUser);
        
        bool isFragmentRedirect = bankProfileEnum != BankProfileEnum.Hsbc_Sandbox;
        if (isFragmentRedirect)
        {
            // Wait for redirect (allow 60s for now to support interaction)
            await page.WaitForSelectorAsync(
                "id=auth-fragment-redirect",
                new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Attached,
                    Timeout = 60000
                });

            // Wait for delegate API call (allows 5s)
            IJSHandle pageStatusJsHandle = await page.WaitForFunctionAsync(
                "window.pageStatus",
                null,
                new PageWaitForFunctionOptions { Timeout = 5000 });

            var pageStatus = await pageStatusJsHandle.JsonValueAsync<string>();
            if (pageStatus is not "POST of fragment succeeded")
            {
                throw new Exception("Redirect page could not capture and pass on parameters in URL fragment");
            }
        }
        else
        {
            // Wait for network activity to complete
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Delay to ensure token available
        await Task.Delay(250);
    }
}
