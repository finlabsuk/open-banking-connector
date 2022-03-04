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
        typeof(HttpResponse<DomesticVrpConsentResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticVrpConsentResponse>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<DomesticVrpConsentResponse>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] DomesticVrpConsent request)
    {
        IFluentResponse<DomesticVrpConsentResponse> fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .CreateAsync(request);

        // HTTP response
        HttpResponse<DomesticVrpConsentResponse> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<DomesticVrpConsentResponse> _ => StatusCodes.Status201Created,
            FluentBadRequestErrorResponse<DomesticVrpConsentResponse> _ => StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<DomesticVrpConsentResponse> _ => StatusCodes.Status500InternalServerError,
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
        typeof(HttpResponse<IList<DomesticVrpConsentResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticVrpConsentResponse>>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<DomesticVrpConsentResponse>>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync()
    {
        // Operation
        IFluentResponse<IQueryable<DomesticVrpConsentResponse>> fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .ReadLocalAsync(query => true);

        // HTTP response
        HttpResponse<IQueryable<DomesticVrpConsentResponse>> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<IQueryable<DomesticVrpConsentResponse>> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<IQueryable<DomesticVrpConsentResponse>> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<IQueryable<DomesticVrpConsentResponse>> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
