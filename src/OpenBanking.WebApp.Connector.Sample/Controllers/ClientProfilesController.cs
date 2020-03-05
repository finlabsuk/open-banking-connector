// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers
{
    [ApiController]
    public class ClientProfilesController : ControllerBase
    {
        private readonly IKeySecretProvider _keySecrets;
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;

        public ClientProfilesController(IOpenBankingRequestBuilder obRequestBuilder, IKeySecretProvider keySecrets)
        {
            _obRequestBuilder = obRequestBuilder;
            _keySecrets = keySecrets;
        }

        [Route("client-profiles")]
        [HttpPost]
        [ProducesResponseType(typeof(PostOpenBankingClientProfileResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessagesResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClientProfilesPostAsync([FromBody] BankClientProfile request)
        {
            var clientResp = await _obRequestBuilder.BankClientProfile()
                .Data(request)
                .SubmitAsync();

            var result = new PostOpenBankingClientProfileResponse(
                clientResp.Data,
                clientResp.ToMessagesResponse()
            );

            return clientResp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
