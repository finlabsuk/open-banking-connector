// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers
{
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly IRequestBuilder _requestBuilder;

        public BanksController(IRequestBuilder requestBuilder)
        {
            _requestBuilder = requestBuilder;
        }

        /// <summary>
        ///     Create a Bank object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [Route("banks")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<BankResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<BankResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<BankResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] Bank request)
        {
            // Operation
            IFluentResponse<BankResponse> fluentResponse = await _requestBuilder
                .BankConfiguration
                .Banks
                .CreateLocalAsync(request);

            // HTTP response
            HttpResponse<BankResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<BankResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<BankResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<BankResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }

        /// <summary>
        ///     Read all Bank objects
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [Route("banks")]
        [HttpGet]
        [ProducesResponseType(
            typeof(HttpResponse<IList<BankResponse>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<BankResponse>>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<BankResponse>>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {
            // Operation
            IFluentResponse<IQueryable<BankResponse>> fluentResponse = await _requestBuilder
                .BankConfiguration
                .Banks
                .GetLocalAsync(query => true);

            // HTTP response
            HttpResponse<IQueryable<BankResponse>> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<IQueryable<BankResponse>> _ => StatusCodes.Status200OK,
                FluentBadRequestErrorResponse<IQueryable<BankResponse>> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<IQueryable<BankResponse>> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
