// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "test")]
[Tags("Account Access Consent Auth Contexts")]
[Route("aisp/account-access-consents")]
public class TestingAccountAccessConsentsController : ControllerBase
{
    public static string BrowserCookieKey => "__Host-Session-Id";

    /// <summary>
    ///     Create account access consent auth context and redirect to auth URL
    /// </summary>
    /// <param name="accountAccessConsentId">ID of account access consent</param>
    /// <param name="modifiedBy"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    [HttpGet("{accountAccessConsentId:guid}/auth")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<ActionResult> GetAsync(
        Guid accountAccessConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "reference")]
        string? reference)
    {
        // Create auth context
        if (TestingMethods.Instance.CreateAccountAccessConsentAuthContext is null)
        {
            throw new InvalidOperationException();
        }
        AccountAccessConsentAuthContextCreateResponse authContextResponse =
            await TestingMethods.Instance.CreateAccountAccessConsentAuthContext(accountAccessConsentId);

        // Set cookie
        Response.Cookies.Append(
            BrowserCookieKey,
            authContextResponse.AppSessionId,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax, // require cookie in OAuth2 redirect
                MaxAge = TimeSpan.FromMinutes(10),
                Secure = true
            });

        return Redirect(authContextResponse.AuthUrl);
    }
}
