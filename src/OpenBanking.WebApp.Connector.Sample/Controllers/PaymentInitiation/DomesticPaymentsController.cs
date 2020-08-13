// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.PaymentInitiation
{
    [ApiController]
    public class DomesticPaymentsController : ControllerBase
    {
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;

        public DomesticPaymentsController(IOpenBankingRequestBuilder obRequestBuilder)
        {
            _obRequestBuilder = obRequestBuilder;
        }

        [Route("pisp/domestic-payments")]
        [HttpPost]
        [ProducesResponseType(type: typeof(PaymentResponse), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(MessagesResponse), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DomesticPaymentsPostAsync([FromBody] DomesticPayment request)
        {
            DomesticPaymentFluentResponse? resp = await _obRequestBuilder.DomesticPayment()
                .Data(request)
                .SubmitAsync();

            PaymentResponse? result = new PaymentResponse(
                messages: resp.ToMessagesResponse(),
                data: resp.Data);

            return resp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
