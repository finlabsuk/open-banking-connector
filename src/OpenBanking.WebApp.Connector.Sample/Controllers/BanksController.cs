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
    public class BanksController : ControllerBase
    {
        private readonly IRequestBuilder _obcRequestBuilder;

        public BanksController(IRequestBuilder obcRequestBuilder)
        {
            _obcRequestBuilder = obcRequestBuilder;
        }

        [Route("banks")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<BankPostResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<BankPostResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<BankPostResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] Bank request)
        {
            // Operation
            IFluentResponse<BankPostResponse> fluentResponse = await _obcRequestBuilder.ClientRegistration
                .Banks
                .PostAsync(request);

            // HTTP response
            HttpResponse<BankPostResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<BankPostResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<BankPostResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<BankPostResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
