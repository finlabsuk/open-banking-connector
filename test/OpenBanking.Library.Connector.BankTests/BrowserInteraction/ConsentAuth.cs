// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Mail;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction.BankUiMethods;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.Playwright;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;

public class ConsentAuth
{
    private readonly IBankProfileService _bankProfileService;
    private readonly EmailOptions _emailOptions;
    private readonly BrowserTypeLaunchOptions _launchOptions;
    private readonly SmtpClient _smtpClient;

    public ConsentAuth(
        BrowserTypeLaunchOptions launchOptions,
        EmailOptions emailOptions,
        IBankProfileService bankProfileService)
    {
        _launchOptions = launchOptions;
        _emailOptions = emailOptions;
        _bankProfileService = bankProfileService;
        _smtpClient = new SmtpClient
        {
            Host = _emailOptions.SmtpServer,
            Port = _emailOptions.SmtpPort,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(_emailOptions.FromEmailAddress, _emailOptions.FromEmailPassword),
            Timeout = 10000
        };
    }

    public async Task<AuthContextUpdateAuthResultResponse> PerformAuth(
        RedirectObserver redirectObserver,
        string authUrl,
        bool useScaAuth,
        OAuth2ResponseMode defaultResponseMode,
        BankUser? bankUser,
        BankProfileEnum bankProfileEnum,
        ConsentVariety consentVariety)
    {
        // Subscribe to "catch" redirects
        using IDisposable sub = RedirectObservable.Instance.Subscribe(redirectObserver);

        // Use email or automated auth
        Task<AuthContextUpdateAuthResultResponse> waitForRedirect;
        if (useScaAuth)
        {
            // Perform email auth
            waitForRedirect = redirectObserver.WaitForRedirect(TimeSpan.FromSeconds(180));
            SendEmail(
                "Open Banking Connector Test",
                "This is the auth URL: " + authUrl);
        }
        else
        {
            if (bankUser is null)
            {
                throw new ArgumentException("No user specified for consent auth.");
            }

            // Perform automated auth
            waitForRedirect = redirectObserver.WaitForRedirect(TimeSpan.FromSeconds(120));
            await AutomatedAuthAsync(
                authUrl,
                bankProfileEnum,
                consentVariety,
                bankUser,
                defaultResponseMode);
        }

        // Await redirect
        return await waitForRedirect;
    }

    private IBankUiMethods GetBankGroupUiMethods(BankProfileEnum bankProfileEnum)
    {
        return BankProfileService.GetBankGroupEnum(bankProfileEnum) switch
        {
            BankGroupEnum.Barclays => new BarclaysUiMethods(_bankProfileService.GetBank<BarclaysBank>(bankProfileEnum)),
            BankGroupEnum.Cooperative => new CooperativeUiMethods(
                _bankProfileService.GetBank<CooperativeBank>(bankProfileEnum)),
            BankGroupEnum.Danske => new DanskeUiMethods(_bankProfileService.GetBank<DanskeBank>(bankProfileEnum)),
            BankGroupEnum.Hsbc => new HsbcUiMethods(_bankProfileService.GetBank<HsbcBank>(bankProfileEnum)),
            BankGroupEnum.Lloyds => new LloydsUiMethods(_bankProfileService.GetBank<LloydsBank>(bankProfileEnum)),
            BankGroupEnum.Obie => new ObieUiMethods(_bankProfileService.GetBank<ObieBank>(bankProfileEnum)),
            BankGroupEnum.Monzo => new MonzoUiMethods(_bankProfileService.GetBank<MonzoBank>(bankProfileEnum)),
            BankGroupEnum.NatWest => new NatWestUiMethods(_bankProfileService.GetBank<NatWestBank>(bankProfileEnum)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task AutomatedAuthAsync(
        string authUrl,
        BankProfileEnum bankProfileEnum,
        ConsentVariety consentVariety,
        BankUser bankUser,
        OAuth2ResponseMode defaultResponseMode)
    {
        IBankUiMethods bankUiMethods = GetBankGroupUiMethods(bankProfileEnum);
        using IPlaywright playwright = await Playwright.CreateAsync();
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(_launchOptions);
        IPage page = await browser.NewPageAsync();
        await page.GotoAsync(authUrl);
        await bankUiMethods.PerformConsentAuthUiInteractions(consentVariety, page, bankUser);

        bool isFragmentRedirect = defaultResponseMode is OAuth2ResponseMode.Fragment;
        if (isFragmentRedirect)
        {
            // Wait for redirect
            var redirectTimeout = 10000;
            await page.WaitForSelectorAsync(
                "id=auth-fragment-redirect",
                new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Attached,
                    Timeout = redirectTimeout
                });

            // Wait for delegate API call
            IJSHandle pageStatusJsHandle = await page.WaitForFunctionAsync(
                "window.pageStatus",
                null,
                new PageWaitForFunctionOptions { Timeout = 0 });

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

            // Seems necessary to give additional delay as sometimes this point is hit before query redirect has been processed
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    private void SendEmail(string subject, string body)
    {
        using var mailMessage = new MailMessage(
            new MailAddress(_emailOptions.FromEmailAddress, _emailOptions.FromEmailName),
            new MailAddress(_emailOptions.ToEmailAddress, _emailOptions.ToEmailName));
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        _smtpClient.Send(mailMessage);
    }
}
