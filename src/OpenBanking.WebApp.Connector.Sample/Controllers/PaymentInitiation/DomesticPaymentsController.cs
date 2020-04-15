// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
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
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessagesResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DomesticPaymentsPostAsync([FromBody] DomesticPaymentRequest request)
        {
            var resp = await _obRequestBuilder.DomesticPayment()
                .Data(request)
                .SubmitAsync();

            var result = new PaymentResponse(
                resp.ToMessagesResponse(),
                resp.Data
            );

            return resp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
