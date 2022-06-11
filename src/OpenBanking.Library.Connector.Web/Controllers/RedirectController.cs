// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "auth-contexts")]
    [Tags("OAuth2 Redirects")]
    [Route("auth")]
    public class RedirectController : ControllerBase
    {
        private readonly IRequestBuilder _requestBuilder;

        public RedirectController(IRequestBuilder requestBuilder)
        {
            _requestBuilder = requestBuilder;
        }

        /// <summary>
        ///     OAuth2 query redirect endpoint
        /// </summary>
        /// <param name="idToken"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        [HttpGet("query-redirect")]
        public async Task<IActionResult> GetQueryRedirectDelegateAsync(
            [FromQuery(Name = "id_token")] string idToken,
            [FromQuery(Name = "code")]
            string code,
            [FromQuery(Name = "state")]
            string state,
            [FromQuery(Name = "nonce")]
            string? nonce)
        {
            // Operation
            var authResult =
                new AuthResult(
                    OAuth2ResponseMode.Query,
                    new OAuth2RedirectData(idToken, code, state, nonce));
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
        /// <param name="nonce"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [HttpPost("redirect-delegate")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthContextUpdateAuthResultResponse))]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> PostRedirectDelegateAsync(
            [FromForm(Name = "id_token")] string idToken,
            [FromForm(Name = "code")]
            string code,
            [FromForm(Name = "state")]
            string state,
            [FromForm(Name = "response_mode")]
            string responseMode,
            [FromForm(Name = "nonce")]
            string? nonce,
            [FromForm(Name = "modified_by")]
            string? modifiedBy)
        {
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
                    new OAuth2RedirectData(idToken, code, state, nonce));
            AuthContextUpdateAuthResultResponse fluentResponse = await _requestBuilder
                .AuthContexts
                .UpdateAuthResultAsync(authResult, modifiedBy);

            return Created("about:blank", fluentResponse);
        }
    }
}
