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

        [HttpPost]
        [Route("auth/fragment-redirect-delegate")]
        [ProducesResponseType(
            typeof(HttpResponseMessages),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponseMessages),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponseMessages),
            StatusCodes.Status500InternalServerError)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> PostAuthorisationCallbackAsync([FromForm] AuthorisationCallbackPayload payload)
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
