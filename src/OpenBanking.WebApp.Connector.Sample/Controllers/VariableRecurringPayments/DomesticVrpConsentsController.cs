// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.VariableRecurringPayments
{
    [ApiController]
    public class DomesticVrpConsentsController : ControllerBase
    {
        private readonly IRequestBuilder _obcRequestBuilder;

        public DomesticVrpConsentsController(IRequestBuilder obcRequestBuilder)
        {
            _obcRequestBuilder = obcRequestBuilder;
        }

        [Route("vrp/domestic-vrp-consents")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticVrpConsentResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticVrpConsentResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticVrpConsentResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] DomesticVrpConsent request)
        {
            IFluentResponse<DomesticVrpConsentResponse> fluentResponse = await _obcRequestBuilder
                .VariableRecurringPayments
                .DomesticVrpConsents
                .PostAsync(request);

            // HTTP response
            HttpResponse<DomesticVrpConsentResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<DomesticVrpConsentResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<DomesticVrpConsentResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<DomesticVrpConsentResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
