// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;

public class ConsentAuth
{
    private readonly IBankProfileService _bankProfileService;
    private readonly BrowserTypeLaunchOptions _launchOptions;

    public ConsentAuth(BrowserTypeLaunchOptions launchOptions, IBankProfileService bankProfileService)
    {
        _launchOptions = launchOptions;
        _bankProfileService = bankProfileService;
    }

    private IBankUiMethods GetBankGroupUiMethods(BankProfileEnum bankProfileEnum)
    {
        return _bankProfileService.GetBankGroupEnum(bankProfileEnum) switch
        {
            BankGroupEnum.Danske => new DanskeUiMethods(_bankProfileService.GetBank<DanskeBank>(bankProfileEnum)),
            BankGroupEnum.Hsbc => new HsbcUiMethods(_bankProfileService.GetBank<HsbcBank>(bankProfileEnum)),
            BankGroupEnum.Lloyds => new LloydsUiMethods(_bankProfileService.GetBank<LloydsBank>(bankProfileEnum)),
            BankGroupEnum.Obie => new ObieUiMethods(_bankProfileService.GetBank<ObieBank>(bankProfileEnum)),
            BankGroupEnum.Monzo => new MonzoUiMethods(_bankProfileService.GetBank<MonzoBank>(bankProfileEnum)),
            BankGroupEnum.NatWest => new NatWestUiMethods(_bankProfileService.GetBank<NatWestBank>(bankProfileEnum)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task AuthoriseAsync(
        string authUrl,
        BankProfile bankProfile,
        ConsentVariety consentVariety,
        BankUser bankUser)
    {
        IBankUiMethods bankUiMethods = GetBankGroupUiMethods(bankProfile.BankProfileEnum);

        using IPlaywright playwright = await Playwright.CreateAsync();

        bool requiresManualInteraction = bankProfile.SupportsSca;
        if (requiresManualInteraction)
        {
            _launchOptions.Headless = false;
        }

        await using IBrowser browser = await playwright.Chromium.LaunchAsync(_launchOptions);
        IPage page = await browser.NewPageAsync();
        await page.GotoAsync(authUrl);
        await bankUiMethods.PerformConsentAuthUiInteractions(consentVariety, page, bankUser);

        bool isFragmentRedirect = bankProfile.DefaultResponseMode is OAuth2ResponseMode.Fragment;
        if (isFragmentRedirect)
        {
            // Wait for redirect
            int redirectTimeout = requiresManualInteraction ? 60000 : 10000; // allow 50s for manual interaction
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
