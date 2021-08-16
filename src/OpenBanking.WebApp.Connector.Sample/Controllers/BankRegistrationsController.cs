// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers
{
    [ApiController]
    public class BankRegistrationsController : ControllerBase
    {
        private readonly IRequestBuilder _obcRequestBuilder;

        public BankRegistrationsController(IRequestBuilder obcRequestBuilder)
        {
            _obcRequestBuilder = obcRequestBuilder;
        }

        [Route("bank-registrations")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<BankRegistrationResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<BankRegistrationResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<BankRegistrationResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] BankRegistration request)
        {
            // Operation
            IFluentResponse<BankRegistrationResponse> fluentResponse = await _obcRequestBuilder
                .ClientRegistration
                .BankRegistrations
                .PostAsync(request);

            // HTTP response
            HttpResponse<BankRegistrationResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<BankRegistrationResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<BankRegistrationResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<BankRegistrationResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
