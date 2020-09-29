// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.PaymentInitiation
{
    [ApiController]
    public class DomesticPaymentConsentsController : ControllerBase
    {
        private readonly IRequestBuilder _obRequestBuilder;

        public DomesticPaymentConsentsController(IRequestBuilder obRequestBuilder)
        {
            _obRequestBuilder = obRequestBuilder;
        }

        [Route("pisp/domestic-payment-consents")]
        [HttpPost]
        [ProducesResponseType(
            type: typeof(HttpResponse<DomesticPaymentConsentResponse>),
            statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(
            type: typeof(MessagesResponse),
            statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DomesticPaymentConsentsPostAsync([FromBody] DomesticPaymentConsent request)
        {
            FluentResponse<DomesticPaymentConsentResponse> resp = await _obRequestBuilder
                .DomesticPaymentConsents
                .PostAsync(request);

            HttpResponse<DomesticPaymentConsentResponse> result = new HttpResponse<DomesticPaymentConsentResponse>(
                messages: resp.ToMessagesResponse(),
                data: resp.Data);

            return resp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
