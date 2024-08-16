// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
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
            var authResult =
                new AuthResult
                {
                    ResponseMode = OAuth2ResponseMode.Query,
                    State = state,
                    OAuth2RedirectOptionalParameters = new OAuth2RedirectOptionalParameters
                    {
                        Error = error,
                        IdToken = idToken,
                        Code = code
                    }
                };
            _ = await _requestBuilder
                .AuthContexts
                .UpdateAuthResultAsync(authResult, "Redirect to /auth/query-redirect");
        }
        else
        {
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
        }

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

        AuthContextUpdateAuthResultResponse fluentResponse;
        if (TestingMethods.Instance.ProcessRedirect is null)
        {
            // Parse response_mode (ASP.NET model binding will only do simple conversion)
            OAuth2ResponseMode? oAuth2ResponseMode = responseMode switch
            {
                null => null,
                "query" => OAuth2ResponseMode.Query,
                "fragment" => OAuth2ResponseMode.Fragment,
                "form_post" => OAuth2ResponseMode.FormPost,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(responseMode),
                    responseMode,
                    "Unknown value for response_mode.")
            };

            // Operation
            var authResult =
                new AuthResult
                {
                    ResponseMode = oAuth2ResponseMode,
                    RedirectUrl = redirectUrl,
                    AppSessionId = appSessionId,
                    State = state,
                    OAuth2RedirectOptionalParameters = new OAuth2RedirectOptionalParameters
                    {
                        Error = error,
                        IdToken = idToken,
                        Code = code
                    }
                };
            fluentResponse = await _requestBuilder
                .AuthContexts
                .UpdateAuthResultAsync(authResult);
        }
        else
        {
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
            fluentResponse = await TestingMethods.Instance.ProcessRedirect(authResult);
        }

        return Created("about:blank", fluentResponse);
    }
}
