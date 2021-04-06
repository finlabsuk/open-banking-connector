﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.WebHost.Models.Public.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers.PaymentInitiation
{
    [ApiController]
    public class DomesticPaymentsController : ControllerBase
    {
        private readonly IRequestBuilder _obcRequestBuilder;

        public DomesticPaymentsController(IRequestBuilder obcRequestBuilder)
        {
            _obcRequestBuilder = obcRequestBuilder;
        }

        [Route("pisp/domestic-payments")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentPostResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentPostResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentPostResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] DomesticPayment request)
        {
            IFluentResponse<DomesticPaymentPostResponse> fluentResponse = await _obcRequestBuilder.PaymentInitiation
                .DomesticPayments
                .PostAsync(request);

            // HTTP response
            HttpResponse<DomesticPaymentPostResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<DomesticPaymentPostResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<DomesticPaymentPostResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<DomesticPaymentPostResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
