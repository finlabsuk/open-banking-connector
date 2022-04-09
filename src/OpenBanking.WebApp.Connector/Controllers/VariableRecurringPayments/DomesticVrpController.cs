// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.VariableRecurringPayments;

[ApiController]
[ApiExplorerSettings(GroupName = "vrp")]
public class DomesticVrpController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public DomesticVrpController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create a DomesticVrp object
    /// </summary>
    /// <param name="domesticVrpConsentId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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
    public async Task<IActionResult> PostAsync(
        [FromHeader(Name = "x-obc-domestic-vrp-consent-id")] [Required] Guid domesticVrpConsentId,
        [FromBody] [Required] DomesticVrp request)
    {
        IFluentResponse<DomesticVrpResponse> fluentResponse = await _requestBuilder.VariableRecurringPayments
            .DomesticVrps
            .CreateAsync(request, domesticVrpConsentId);

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
}
