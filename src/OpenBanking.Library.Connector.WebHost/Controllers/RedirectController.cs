// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(MessagesResponse), statusCode: StatusCodes.Status400BadRequest)]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> PostAuthorisationCallbackAsync([FromForm] AuthorisationCallbackPayload payload)
        {
            AuthorisationRedirectObject value = new AuthorisationRedirectObject(
                responseMode: "fragment",
                response: payload);

            FluentResponse<AuthorisationRedirectObjectResponse> resp = await _obRequestBuilder
                .AuthorisationRedirectObjects
                .PostAsync(value);

            return resp.HasErrors
                ? new BadRequestObjectResult(resp.ToMessagesResponse()) as IActionResult
                : new NoContentResult();
        }
    }
}
