﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Controllers.AccountAndTransaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Controllers.AuthContext;

[ApiController]
[ApiExplorerSettings(GroupName = "test")]
[Tags("Post-Auth Redirects")]
[Route("auth")]
public class TestingRedirectsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public TestingRedirectsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     OAuth2 query redirect endpoint
    /// </summary>
    /// <param name="idToken"></param>
    /// <param name="code"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    [HttpGet("query-redirect")]
    public async Task<ActionResult> QueryRedirectGetAsync(
        [FromQuery(Name = "id_token")]
        string idToken,
        [FromQuery(Name = "code")]
        string code,
        [FromQuery(Name = "state")]
        string state)
    {
        // Operation
        var authResult =
            new AuthResult(
                OAuth2ResponseMode.Query,
                null,
                null,
                new OAuth2RedirectData(idToken, code, state));
        _ = await _requestBuilder
            .AuthContexts
            .UpdateAuthResultAsync(authResult, "Redirect to /auth/query-redirect");

        return Ok(); // We do not return data to bank following query redirect
    }

    /// <summary>
    ///     Delegate endpoint for forwarding data captured elsewhere from OAuth2 redirect
    /// </summary>
    /// <param name="idToken"></param>
    /// <param name="code"></param>
    /// <param name="state"></param>
    /// <param name="responseMode"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [HttpPost("fragment-redirect-delegate")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthContextUpdateAuthResultResponse>> FragmentRedirectDelegatePostAsync(
        [FromForm(Name = "id_token")]
        string idToken,
        [FromForm(Name = "code")]
        string code,
        [FromForm(Name = "state")]
        string state,
        [FromForm(Name = "response_mode")]
        string responseMode,
        [FromForm(Name = "modified_by")]
        string? modifiedBy,
        [FromForm(Name = "redirect_uri")]
        string? redirectUrl)
    {
        // Check for cookie
        string cookieKey = TestingAccountAccessConsentsController.BrowserCookieKey;
        Request.Cookies.TryGetValue(cookieKey, out string? appSessionId);

        // Operation
        OAuth2ResponseMode oAuth2ResponseMode = responseMode switch
        {
            "query" => OAuth2ResponseMode.Query,
            "fragment" => OAuth2ResponseMode.Fragment,
            "form_post" => OAuth2ResponseMode.FormPost,
            _ => throw new ArgumentOutOfRangeException(
                nameof(responseMode),
                responseMode,
                "Unknown value for response_mode.")
        };
        var authResult =
            new AuthResult(
                oAuth2ResponseMode,
                redirectUrl,
                appSessionId,
                new OAuth2RedirectData(idToken, code, state));
        AuthContextUpdateAuthResultResponse fluentResponse = await _requestBuilder
            .AuthContexts
            .UpdateAuthResultAsync(authResult, modifiedBy);

        return Created("about:blank", fluentResponse);
    }
}
