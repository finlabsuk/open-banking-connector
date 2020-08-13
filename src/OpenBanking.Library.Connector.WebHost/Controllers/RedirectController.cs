// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.Library.Connector.WebHost.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;

        public RedirectController(IOpenBankingRequestBuilder obRequestBuilder)
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
            AuthorisationCallbackData? value = new AuthorisationCallbackData(
                responseMode: "fragment",
                response: payload);

            AuthorisationCallbackDataFluentResponse? resp = await _obRequestBuilder.AuthorisationCallbackData()
                .Data(value)
                .SubmitAsync();

            return resp.HasErrors
                ? new BadRequestObjectResult(resp.ToMessagesResponse()) as IActionResult
                : new NoContentResult();
        }
    }
}
