// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.PaymentInitiation
{
    [ApiController]
    public class PaymentInitiationApiProfilesController : ControllerBase
    {
        private readonly IKeySecretReadOnlyProvider _keySecrets;
        private readonly IRequestBuilder _obRequestBuilder;

        public PaymentInitiationApiProfilesController(
            IRequestBuilder obRequestBuilder,
            IKeySecretReadOnlyProvider keySecrets)
        {
            _obRequestBuilder = obRequestBuilder;
            _keySecrets = keySecrets;
        }

        [Route("pisp/api-profiles")]
        [HttpPost]
        [ProducesResponseType(
            type: typeof(HttpResponse<BankProfileResponse>),
            statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(MessagesResponse), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClientProfilesPostAsync([FromBody] BankProfile request)
        {
            FluentResponse<BankProfileResponse> clientResp = await _obRequestBuilder.ClientRegistration
                .BankProfiles
                .PostAsync(request);

            HttpResponse<BankProfileResponse> result = new HttpResponse<BankProfileResponse>(
                data: clientResp.Data,
                messages: clientResp.ToMessagesResponse());

            return clientResp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
