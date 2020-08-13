// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers
{
    [ApiController]
    public class BankClientProfilesController : ControllerBase
    {
        private readonly IKeySecretReadOnlyProvider _keySecrets;
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;

        public BankClientProfilesController(
            IOpenBankingRequestBuilder obRequestBuilder,
            IKeySecretReadOnlyProvider keySecrets)
        {
            _obRequestBuilder = obRequestBuilder;
            _keySecrets = keySecrets;
        }

        [Route("bank-client-profiles")]
        [HttpPost]
        [ProducesResponseType(type: typeof(BankClientProfileHttpResponse), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(MessagesResponse), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClientProfilesPostAsync([FromBody] BankClientProfile request)
        {
            BankClientProfileFluentResponse? clientResp = await _obRequestBuilder.BankClientProfile()
                .Data(request)
                .SubmitAsync();

            BankClientProfileHttpResponse? result = new BankClientProfileHttpResponse(
                data: clientResp.Data,
                messages: clientResp.ToMessagesResponse());

            return clientResp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
