// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers
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
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(MessagesResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostAuthorisationCallbackAsync([FromBody] AuthorisationCallbackData value)
        {
            var resp = await _obRequestBuilder.AuthorisationCallbackData()
                .Data(value)
                .SubmitAsync();

            return resp.HasErrors
                ? new BadRequestObjectResult(resp.ToMessagesResponse()) as IActionResult
                : new NoContentResult();
        }
    }
}
