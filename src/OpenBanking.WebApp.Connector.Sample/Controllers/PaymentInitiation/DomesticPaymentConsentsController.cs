// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities.PaymentInitiation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.PaymentInitiation
{
    [ApiController]
    public class DomesticPaymentConsentsController : ControllerBase
    {
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;

        public DomesticPaymentConsentsController(IOpenBankingRequestBuilder obRequestBuilder)
        {
            _obRequestBuilder = obRequestBuilder;
        }

        [Route("pisp/domestic-payment-consents")]
        [HttpPost]
        [ProducesResponseType(
            type: typeof(DomesticPaymentConsentHttpResponse),
            statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(
            type: typeof(MessagesResponse),
            statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DomesticPaymentConsentsPostAsync([FromBody] OBWriteDomesticConsent request)
        {
            DomesticPaymentConsentFluentResponse? resp = await _obRequestBuilder
                .DomesticPaymentConsent(request.ApiProfileId)
                .Data(request)
                .SubmitAsync();

            DomesticPaymentConsentHttpResponse? result = new DomesticPaymentConsentHttpResponse(
                messages: resp.ToMessagesResponse(),
                data: resp.Data);

            return resp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
