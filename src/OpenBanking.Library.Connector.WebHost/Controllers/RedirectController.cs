// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.WebHost.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IRequestBuilder _obRequestBuilder;

        public RedirectController(IRequestBuilder obRequestBuilder)
        {
            _obRequestBuilder = obRequestBuilder;
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
            AuthorisationRedirectObject value = new AuthorisationRedirectObject(
                "fragment",
                payload.ToLibraryVersion());
            IFluentResponse<AuthorisationRedirectObjectResponse> fluentResponse = await _obRequestBuilder
                .PaymentInitiation
                .AuthorisationRedirectObjects
                .PostAsync(value);

            // HTTP response
            HttpResponse<AuthorisationRedirectObjectResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<AuthorisationRedirectObjectResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<AuthorisationRedirectObjectResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<AuthorisationRedirectObjectResponse> _ => StatusCodes
                    .Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
