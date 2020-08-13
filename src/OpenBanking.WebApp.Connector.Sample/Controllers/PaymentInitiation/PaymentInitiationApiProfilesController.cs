// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities.PaymentInitiation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.PaymentInitiation
{
    [ApiController]
    public class PaymentInitiationApiProfilesController : ControllerBase
    {
        private readonly IKeySecretReadOnlyProvider _keySecrets;
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;

        public PaymentInitiationApiProfilesController(
            IOpenBankingRequestBuilder obRequestBuilder,
            IKeySecretReadOnlyProvider keySecrets)
        {
            _obRequestBuilder = obRequestBuilder;
            _keySecrets = keySecrets;
        }

        [Route("pisp/api-profiles")]
        [HttpPost]
        [ProducesResponseType(
            type: typeof(PaymentInitiationApiProfileHttpResponse),
            statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(MessagesResponse), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClientProfilesPostAsync([FromBody] PaymentInitiationApiProfile request)
        {
            PaymentInitiationApiProfileFluentResponse? clientResp = await _obRequestBuilder
                .PaymentInitiationApiProfile()
                .Data(request)
                .SubmitAsync();

            PaymentInitiationApiProfileHttpResponse? result = new PaymentInitiationApiProfileHttpResponse(
                data: clientResp.Data,
                messages: clientResp.ToMessagesResponse());

            return clientResp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
