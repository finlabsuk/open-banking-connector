// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers
{
    [ApiController]
    public class BankApiInformationRecordsController : ControllerBase
    {
        private readonly IRequestBuilder _obcRequestBuilder;

        public BankApiInformationRecordsController(IRequestBuilder obcRequestBuilder)
        {
            _obcRequestBuilder = obcRequestBuilder;
        }

        [Route("bank-api-information-records")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<BankApiInformationResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<BankApiInformationResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<BankApiInformationResponse>),
            StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseMessages), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] BankApiInformation request)
        {
            IFluentResponse<BankApiInformationResponse> fluentResponse = await _obcRequestBuilder
                .ClientRegistration
                .BankApiInformationObjects
                .PostLocalAsync(request);

            // HTTP response
            HttpResponse<BankApiInformationResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<BankApiInformationResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<BankApiInformationResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<BankApiInformationResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
