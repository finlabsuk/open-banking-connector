// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
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
            typeof(HttpResponse<AuthorisationRedirectObjectResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<AuthorisationRedirectObjectResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<AuthorisationRedirectObjectResponse>),
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
            HttpResponse<AuthContextResponse> httpResponse =
                fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<AuthContextResponse> _ => StatusCodes
                    .Status201Created,
                FluentBadRequestErrorResponse<AuthContextResponse> _ => StatusCodes
                    .Status400BadRequest,
                FluentOtherErrorResponse<AuthContextResponse> _ => StatusCodes
                    .Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
