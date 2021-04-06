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
    public class DomesticPaymentConsentsController : ControllerBase
    {
        private readonly IRequestBuilder _obcRequestBuilder;

        public DomesticPaymentConsentsController(IRequestBuilder obcRequestBuilder)
        {
            _obcRequestBuilder = obcRequestBuilder;
        }

        [Route("pisp/domestic-payment-consents")]
        [HttpPost]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentConsentPostResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentConsentPostResponse>),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(HttpResponse<DomesticPaymentConsentPostResponse>),
            StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] DomesticPaymentConsent request)
        {
            IFluentResponse<DomesticPaymentConsentPostResponse> fluentResponse = await _obcRequestBuilder
                .PaymentInitiation
                .DomesticPaymentConsents
                .PostAsync(request);

            // HTTP response
            HttpResponse<DomesticPaymentConsentPostResponse> httpResponse = fluentResponse.ToHttpResponse();
            int statusCode = fluentResponse switch
            {
                FluentSuccessResponse<DomesticPaymentConsentPostResponse> _ => StatusCodes.Status201Created,
                FluentBadRequestErrorResponse<DomesticPaymentConsentPostResponse> _ => StatusCodes.Status400BadRequest,
                FluentOtherErrorResponse<DomesticPaymentConsentPostResponse> _ => StatusCodes.Status500InternalServerError,
                _ => throw new ArgumentOutOfRangeException()
            };
            return new ObjectResult(httpResponse)
                { StatusCode = statusCode };
        }
    }
}
