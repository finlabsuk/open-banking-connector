// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.PaymentInitiation
{
    [ApiController]
    public class DomesticPaymentsController : ControllerBase
    {
       private readonly IRequestBuilder _requestBuilder;

        public DomesticPaymentsController(IRequestBuilder requestBuilder)
        {
            _requestBuilder = requestBuilder;
        }
        
        [Route("pisp/domestic-payments")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] DomesticPayment request)
        {
            IFluentResponse<DomesticPaymentResponse> fluentResponse = await _requestBuilder
                .PaymentInitiation
                .DomesticPayments
                .PostAsync(request);

            // HTTP response
            HttpResponse<DomesticPaymentResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<DomesticPaymentResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<DomesticPaymentResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<DomesticPaymentResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
        
        // GET /pisp/domestic-payments
        [Route("pisp/domestic-payments")]
        [HttpGet]
        [ProducesResponseType(
            typeof(HttpResponse<IList<DomesticPaymentResponse>>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<DomesticPaymentResponse>>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<IList<DomesticPaymentResponse>>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {
            // Operation
            IFluentResponse<IQueryable<DomesticPaymentResponse>> fluentResponse = await _requestBuilder
                .PaymentInitiation
                .DomesticPayments
                .GetLocalAsync(query => true);

            // HTTP response
            HttpResponse<IQueryable<DomesticPaymentResponse>> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<IQueryable<DomesticPaymentResponse>> _ => StatusCodes.Status200OK,
                FluentBadRequestErrorResponse<IQueryable<DomesticPaymentResponse>> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<IQueryable<DomesticPaymentResponse>> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
