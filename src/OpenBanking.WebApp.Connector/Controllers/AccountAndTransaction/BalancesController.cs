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
///     HTTP response object used when reading Balances objects. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class BalancesHttpResponse : HttpResponse<BalancesResponse>
{
    public BalancesHttpResponse(HttpResponseMessages? messages, BalancesResponse? data) :
        base(messages, data) { }
}

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
public class BalancesController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public BalancesController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Get Balances
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/balances")]
    [HttpGet]
    [ProducesResponseType(
        typeof(BalancesHttpResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(BalancesHttpResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(BalancesHttpResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] string accountAccessConsentId)
    {
        // Operation
        IFluentResponse<BalancesResponse> fluentResponse = null!;

        // HTTP response
        var httpResponse = (BalancesHttpResponse) fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<BalancesResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<BalancesResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<BalancesResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
