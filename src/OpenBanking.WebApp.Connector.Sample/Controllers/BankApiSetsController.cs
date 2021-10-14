// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BankApiSetsController : ControllerBase
    {
        private readonly IRequestBuilder _requestBuilder;

        public BankApiSetsController(IRequestBuilder requestBuilder)
        {
            _requestBuilder = requestBuilder;
        }

        [Route("bank-api-sets")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<BankApiSetResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<BankApiSetResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<BankApiSetResponse>),
            StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseMessages), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] BankApiSet request)
        {
            IFluentResponse<BankApiSetResponse> fluentResponse = await _requestBuilder
                .BankConfiguration
                .BankApiSets
                .PostLocalAsync(request);

            // HTTP response
            HttpResponse<BankApiSetResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<BankApiSetResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<BankApiSetResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<BankApiSetResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
        
        // GET /bank-api-sets
        [Route("bank-api-sets")]
        [HttpGet]
        [ProducesResponseType(
            typeof(HttpResponse<IList<BankApiSetResponse>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<BankApiSetResponse>>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<BankApiSetResponse>>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {
            // Operation
            IFluentResponse<IQueryable<BankApiSetResponse>> fluentResponse = await _requestBuilder
                .BankConfiguration
                .BankApiSets
                .GetLocalAsync(query => true);

            // HTTP response
            HttpResponse<IQueryable<BankApiSetResponse>> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<IQueryable<BankApiSetResponse>> _ => StatusCodes.Status200OK,
                FluentBadRequestErrorResponse<IQueryable<BankApiSetResponse>> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<IQueryable<BankApiSetResponse>> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
