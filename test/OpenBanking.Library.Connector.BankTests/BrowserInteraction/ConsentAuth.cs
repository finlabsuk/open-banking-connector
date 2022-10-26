﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;

public class ConsentAuth
{
    private readonly IBankProfileDefinitions _bankProfileDefinitions;
    private readonly BrowserTypeLaunchOptions _launchOptions;

    public ConsentAuth(BrowserTypeLaunchOptions launchOptions, IBankProfileDefinitions bankProfileDefinitions)
    {
        _launchOptions = launchOptions;
        _bankProfileDefinitions = bankProfileDefinitions;
    }

    private IBankGroupUiMethods GetBankGroupUiMethods(BankProfileEnum bankProfileEnum)
    {
        return _bankProfileDefinitions.GetBankGroup(_bankProfileDefinitions.GetBankGroupEnum(bankProfileEnum)) switch
        {
            Danske danske => new DanskeUiMethods(danske),
            Hsbc hsbc => new HsbcUiMethods(hsbc),
            Lloyds lloyds => new LloydsUiMethods(lloyds),
            Monzo monzo => new MonzoUiMethods(monzo),
            NatWest natWest => new NatWestUiMethods(natWest),
            Obie obie => new ObieUiMethods(obie),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task AuthoriseAsync(
        string authUrl,
        BankProfileEnum bankProfileEnum,
        ConsentVariety consentVariety,
        BankUser bankUser)
    {
        IBankGroupUiMethods bankGroupUiMethods = GetBankGroupUiMethods(bankProfileEnum);

        using IPlaywright? playwright = await Playwright.CreateAsync();

        bool requiresManualInteraction = bankGroupUiMethods.RequiresManualInteraction(bankProfileEnum, consentVariety);
        if (requiresManualInteraction)
        {
            _launchOptions.Headless = false;
        }

        await using IBrowser browser = await playwright.Chromium.LaunchAsync(_launchOptions);
        IPage page = await browser.NewPageAsync();
        await page.GotoAsync(authUrl);
        await bankGroupUiMethods.PerformConsentAuthUiInteractions(bankProfileEnum, consentVariety, page, bankUser);

        bool isFragmentRedirect = bankProfileEnum != BankProfileEnum.Hsbc_Sandbox;
        if (isFragmentRedirect)
        {
            // Wait for redirect
            int redirectTimeout = requiresManualInteraction ? 60000 : 10000; // allow 60s with manual interaction
            await page.WaitForSelectorAsync(
                "id=auth-fragment-redirect",
                new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Attached,
                    Timeout = redirectTimeout
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
