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

/// <summary>
///     HTTP response object used when reading Parties objects. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class Parties2HttpResponse : HttpResponse<Parties2Response>
{
    public Parties2HttpResponse(HttpResponseMessages? messages, Parties2Response? data) :
        base(messages, data) { }
}

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Parties")]
public class PartiesController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public PartiesController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Read Party
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/party")]
    [Route("aisp/accounts/{externalApiAccountId}/party")]
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
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] Guid accountAccessConsentId,
        string? externalApiAccountId)
    {
        // Operation
        IFluentResponse<PartiesResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Parties
            .ReadAsync(accountAccessConsentId, externalApiAccountId);

        // HTTP response
        HttpResponse<PartiesResponse> httpResponseTmp = fluentResponse.ToHttpResponse();
        var httpResponse = new PartiesHttpResponse(httpResponseTmp.Messages, httpResponseTmp.Data);
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

    /// <summary>
    ///     Read Parties
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/accounts/{externalApiAccountId}/parties")]
    [HttpGet]
    [ProducesResponseType(
        typeof(Parties2HttpResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(Parties2HttpResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(Parties2HttpResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get2Async(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] Guid accountAccessConsentId,
        string? externalApiAccountId)
    {
        // Operation
        IFluentResponse<Parties2Response> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Parties2
            .ReadAsync(accountAccessConsentId, externalApiAccountId);

        // HTTP response
        HttpResponse<Parties2Response> httpResponseTmp = fluentResponse.ToHttpResponse();
        var httpResponse = new Parties2HttpResponse(httpResponseTmp.Messages, httpResponseTmp.Data);
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<Parties2Response> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<Parties2Response> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<Parties2Response> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
