// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.PaymentInitiation;

[ApiController]
[ApiExplorerSettings(GroupName = "pisp")]
public class DomesticPaymentConsentsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public DomesticPaymentConsentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create a DomesticPaymentConsent object
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("pisp/domestic-payment-consents")]
    [HttpPost]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticPaymentConsentReadResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticPaymentConsentReadResponse>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticPaymentConsentReadResponse>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] DomesticPaymentConsent request)
    {
        IFluentResponse<DomesticPaymentConsentReadResponse> fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .CreateAsync(request);

        // HTTP response
        HttpResponse<DomesticPaymentConsentReadResponse> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<DomesticPaymentConsentReadResponse> _ => StatusCodes.Status201Created,
            FluentBadRequestErrorResponse<DomesticPaymentConsentReadResponse> _ => StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<DomesticPaymentConsentReadResponse> _ => StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }

    /// <summary>
    ///     Read all DomesticPaymentConsent objects
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("pisp/domestic-payment-consents")]
    [HttpGet]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticPaymentConsentReadLocalResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticPaymentConsentReadLocalResponse>>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticPaymentConsentReadLocalResponse>>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync()
    {
        // Operation
        IFluentResponse<IQueryable<DomesticPaymentConsentReadLocalResponse>> fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .ReadLocalAsync(query => true);

        // HTTP response
        HttpResponse<IQueryable<DomesticPaymentConsentReadLocalResponse>>
            httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<IQueryable<DomesticPaymentConsentReadLocalResponse>> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<IQueryable<DomesticPaymentConsentReadLocalResponse>> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<IQueryable<DomesticPaymentConsentReadLocalResponse>> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
