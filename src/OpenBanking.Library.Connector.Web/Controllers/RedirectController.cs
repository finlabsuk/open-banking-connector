// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Fapi;
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
        public async Task<IActionResult> PostFragmentRedirectDelegateAsync(
            [FromQuery] AuthorisationCallbackPayloadQuery payload)
        {
            // Operation
            AuthResult authResult = payload.ToLibraryVersion();
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
        [Route("auth/fragment-redirect-delegate")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(HttpResponseMessages))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(HttpResponseMessages))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(HttpResponseMessages))]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> PostFragmentRedirectDelegateAsync(
            [FromForm] AuthorisationCallbackPayload payload)
        {
            // Operation
            AuthResult authResult = payload.ToLibraryVersion();
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
