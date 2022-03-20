// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

/// <summary>
///     HTTP response object used when reading Parties objects. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class PartiesHttpResponse : HttpResponse<PartiesResponse>
{
    public PartiesHttpResponse(HttpResponseMessages? messages, PartiesResponse? data) :
        base(messages, data) { }
}

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
public class PartiesController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public PartiesController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Get Parties
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/parties")]
    [HttpGet]
    [ProducesResponseType(
        typeof(PartiesHttpResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(PartiesHttpResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(PartiesHttpResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] string accountAccessConsentId)
    {
        // Operation
        IFluentResponse<PartiesResponse> fluentResponse = null!;

        // HTTP response
        var httpResponse = (PartiesHttpResponse) fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<PartiesResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<PartiesResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<PartiesResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
