// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.Web.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "auth-contexts")]
    public class RedirectController : ControllerBase
    {
        private readonly IRequestBuilder _requestBuilder;

        public RedirectController(IRequestBuilder requestBuilder)
        {
            _requestBuilder = requestBuilder;
        }

        [HttpGet]
        [Route("auth/query-redirect")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HttpResponseMessages))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(HttpResponseMessages))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpResponseMessages))]
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
            IFluentResponse<AuthContextResponse> fluentResponse = await _requestBuilder
                .AuthContexts
                .UpdateAuthResultLocalAsync(authResult);

            // HTTP response
            return fluentResponse switch
            {
                FluentSuccessResponse<AuthContextResponse> _ =>
                    new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                        { StatusCode = StatusCodes.Status200OK },
                FluentBadRequestErrorResponse<AuthContextResponse> _ =>
                    new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                        { StatusCode = StatusCodes.Status400BadRequest },
                FluentOtherErrorResponse<AuthContextResponse> _ =>
                    new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                        { StatusCode = StatusCodes.Status500InternalServerError },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [HttpPost]
        [Route("auth/redirect-delegate")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(HttpResponseMessages))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(HttpResponseMessages))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpResponseMessages))]
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
            string? nonce)
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
            IFluentResponse<AuthContextResponse> fluentResponse = await _requestBuilder
                .AuthContexts
                .UpdateAuthResultLocalAsync(authResult);

            // HTTP response
            return fluentResponse switch
            {
                FluentSuccessResponse<AuthContextResponse> _ =>
                    new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                        { StatusCode = StatusCodes.Status201Created },
                FluentBadRequestErrorResponse<AuthContextResponse> _ =>
                    new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                        { StatusCode = StatusCodes.Status400BadRequest },
                FluentOtherErrorResponse<AuthContextResponse> _ =>
                    new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                        { StatusCode = StatusCodes.Status500InternalServerError },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
