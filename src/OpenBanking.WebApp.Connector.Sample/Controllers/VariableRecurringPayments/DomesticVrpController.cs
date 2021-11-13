// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DomesticVrpController : ControllerBase
    {
        private readonly IRequestBuilder _requestBuilder;

        public DomesticVrpController(IRequestBuilder requestBuilder)
        {
            _requestBuilder = requestBuilder;
        }

        [Route("vrp/domestic-vrps")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticVrpResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticVrpResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticVrpResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] DomesticVrp request)
        {
            IFluentResponse<DomesticVrpResponse> fluentResponse = await _requestBuilder.VariableRecurringPayments
                .DomesticVrps
                .PostAsync(request);

            // HTTP response
            HttpResponse<DomesticVrpResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<DomesticVrpResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<DomesticVrpResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<DomesticVrpResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }

        // GET /vrp/domestic-vrps
        [Route("vrp/domestic-vrps")]
        [HttpGet]
        [ProducesResponseType(
            typeof(HttpResponse<IList<DomesticVrpResponse>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<DomesticVrpResponse>>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<DomesticVrpResponse>>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {
            // Operation
            IFluentResponse<IQueryable<DomesticVrpResponse>> fluentResponse = await _requestBuilder
                .VariableRecurringPayments
                .DomesticVrps
                .GetLocalAsync(query => true);

            // HTTP response
            HttpResponse<IQueryable<DomesticVrpResponse>> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<IQueryable<DomesticVrpResponse>> _ => StatusCodes.Status200OK,
                FluentBadRequestErrorResponse<IQueryable<DomesticVrpResponse>> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<IQueryable<DomesticVrpResponse>> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
