// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.VariableRecurringPayments;

[ApiController]
[ApiExplorerSettings(GroupName = "vrp")]
public class DomesticVrpConsentsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public DomesticVrpConsentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create a DomesticVrpConsent object
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("vrp/domestic-vrp-consents")]
    [HttpPost]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticVrpConsentReadResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticVrpConsentReadResponse>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticVrpConsentReadResponse>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] DomesticVrpConsent request)
    {
        IFluentResponse<DomesticVrpConsentReadResponse> fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .CreateAsync(request);

        // HTTP response
        HttpResponse<DomesticVrpConsentReadResponse> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<DomesticVrpConsentReadResponse> _ => StatusCodes.Status201Created,
            FluentBadRequestErrorResponse<DomesticVrpConsentReadResponse> _ => StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<DomesticVrpConsentReadResponse> _ => StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }

    /// <summary>
    ///     Return all DomesticVrpConsent objects
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("vrp/domestic-vrp-consents")]
    [HttpGet]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticVrpConsentReadLocalResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticVrpConsentReadLocalResponse>>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticVrpConsentReadLocalResponse>>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync()
    {
        // Operation
        IFluentResponse<IQueryable<DomesticVrpConsentReadLocalResponse>> fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .ReadLocalAsync(query => true);

        // HTTP response
        HttpResponse<IQueryable<DomesticVrpConsentReadLocalResponse>> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<IQueryable<DomesticVrpConsentReadResponse>> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<IQueryable<DomesticVrpConsentReadResponse>> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<IQueryable<DomesticVrpConsentReadResponse>> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
