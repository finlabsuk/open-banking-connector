// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
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

        [Route("domestic-payment-consents")]
        [HttpPost]
        [ProducesResponseType(typeof(PostDomesticConsentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessagesResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DomesticPaymentConsentsPostAsync([FromBody] OBWriteDomesticConsent request)
        {
            var resp = await _obRequestBuilder.DomesticPaymentConsent(request.OpenBankingClientProfileId)
                .Data(request)
                .SubmitAsync();

            var result = new PostDomesticConsentResponse(
                resp.ToMessagesResponse(),
                resp.Data
            );

            return resp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }
    }
}
