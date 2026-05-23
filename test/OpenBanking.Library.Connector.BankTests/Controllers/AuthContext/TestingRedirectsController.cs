// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Controllers.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Controllers.AuthContext;

[ApiController]
[ApiExplorerSettings(GroupName = "test")]
[Tags("Post-Auth Redirects")]
[Route("auth")]
public class TestingRedirectsController : ControllerBase
{
    /// <summary>
    ///     OAuth2 query redirect endpoint
    /// </summary>
    /// <param name="state"></param>
    /// <param name="code"></param>
    /// <param name="idToken"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    [HttpGet("query-redirect")]
    public async Task<ActionResult> QueryRedirectGetAsync(
        [Required] [FromQuery(Name = "state")]
        string state,
        [FromQuery(Name = "code")]
        string? code,
        [FromQuery(Name = "id_token")]
        string? idToken,
        [FromQuery(Name = "error")]
        string? error)
    {
        if (TestingMethods.Instance.ProcessRedirect is null)
        {
            throw new InvalidOperationException();
        }

        // Create form collection from query parameters
        List<KeyValuePair<string, string?>> formCollection = Request.Query
            .SelectMany(
                pair => pair.Value,
                (pair, value) => new KeyValuePair<string, string?>(pair.Key, value))
            .ToList();

        // Add response_mode
        formCollection.Add(new KeyValuePair<string, string?>("response_mode", "query"));
        var authResult =
            new TestingAuthResult
            {
                State = state,
                RedirectParameters = formCollection
            };
        await TestingMethods.Instance.ProcessRedirect(authResult);

        return Ok(); // We do not return data to bank following query redirect
    }

    /// <summary>
    ///     Delegate endpoint for forwarding data captured elsewhere from OAuth2 redirect
    /// </summary>
    /// <param name="state"></param>
    /// <param name="code"></param>
    /// <param name="idToken"></param>
    /// <param name="error"></param>
    /// <param name="responseMode"></param>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [HttpPost("fragment-redirect-delegate")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthContextUpdateAuthResultResponse>> FragmentRedirectDelegatePostAsync(
        [Required] [FromForm(Name = "state")]
        string state,
        [FromForm(Name = "code")]
        string? code,
        [FromForm(Name = "id_token")]
        string? idToken,
        [FromForm(Name = "error")]
        string? error,
        [FromForm(Name = "response_mode")]
        string? responseMode,
        [FromForm(Name = "redirect_uri")]
        string? redirectUrl)
    {
        // Check for cookie
        string cookieKey = TestingAccountAccessConsentsController.BrowserCookieKey;
        Request.Cookies.TryGetValue(cookieKey, out string? appSessionId);

        if (TestingMethods.Instance.ProcessRedirect is null)
        {
            throw new InvalidOperationException();
        }

        // Create form collection
        List<KeyValuePair<string, string?>> formCollection = Request.Form
            .SelectMany(
                pair => pair.Value,
                (pair, value) => new KeyValuePair<string, string?>(pair.Key, value)).ToList();

        // Add app session ID to form collection
        if (appSessionId is not null)
        {
            formCollection.Add(new KeyValuePair<string, string?>("app_session_id", appSessionId));
        }

        var authResult =
            new TestingAuthResult
            {
                State = state,
                RedirectParameters = formCollection
            };
        AuthContextUpdateAuthResultResponse fluentResponse = await TestingMethods.Instance.ProcessRedirect(authResult);

        return Created("about:blank", fluentResponse);
    }
}
