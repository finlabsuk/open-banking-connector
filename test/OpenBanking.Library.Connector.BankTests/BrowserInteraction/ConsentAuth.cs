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
            BankGroupEnum.Barclays => new BarclaysUiMethods(_bankProfileService.GetBank<BarclaysBank>(bankProfileEnum)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task AuthoriseAsync(
        string authUrl,
        BankProfile bankProfile,
        ConsentVariety consentVariety,
        BankUser bankUser,
        Func<Task<bool>> authIsCompleteFcn)
    {
        bool useManualAuth = bankProfile.SupportsSca;
        int maxWaitTimeForConsentMs;
        if (useManualAuth)
        {
            maxWaitTimeForConsentMs = 180000;
            SendEmail(
                "Open Banking Connector Test",
                "This is the auth URL: " + authUrl);
        }
        else
        {
            maxWaitTimeForConsentMs = 10000;
            IBankUiMethods bankUiMethods = GetBankGroupUiMethods(bankProfile.BankProfileEnum);
            using IPlaywright playwright = await Playwright.CreateAsync();
            await using IBrowser browser = await playwright.Chromium.LaunchAsync(_launchOptions);
            IPage page = await browser.NewPageAsync();
            await page.GotoAsync(authUrl);
            await bankUiMethods.PerformConsentAuthUiInteractions(consentVariety, page, bankUser);

            bool isFragmentRedirect = bankProfile.DefaultResponseMode is OAuth2ResponseMode.Fragment;
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

                // Wait for delegate API call (allows 5s)
                IJSHandle pageStatusJsHandle = await page.WaitForFunctionAsync(
                    "window.pageStatus",
                    null,
                    new PageWaitForFunctionOptions { Timeout = 12000 });

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
        }

        // Ensure token available
        //var t = new Timer(o => { }, null, 0, 60000);
        Task checkAuthCompleteTask = Task.Run(
            async () =>
            {
                var authIsComplete = false;
                while (!authIsComplete)
                {
                    Thread.Sleep(1000); // sleep 1s between retries
                    authIsComplete = await authIsCompleteFcn();
                }
            });

        if (!checkAuthCompleteTask.Wait(TimeSpan.FromMilliseconds(maxWaitTimeForConsentMs)))
        {
            throw new TimeoutException(
                $"Consent auth not completed successfully within {maxWaitTimeForConsentMs / 1000} seconds.");
        }
    }

    private void SendEmail(string subject, string body)
    {
        using var mailMessage = new MailMessage(
            new MailAddress(_emailOptions.FromEmailAddress, _emailOptions.FromEmailName),
            new MailAddress(_emailOptions.ToEmailAddress, _emailOptions.ToEmailName))
        {
            Subject = subject,
            Body = body
        };
        _smtpClient.Send(mailMessage);
    }
}
