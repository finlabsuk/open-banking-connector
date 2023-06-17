// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Parties")]
public class PartiesController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public PartiesController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Read party
    /// </summary>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("aisp/party")]
    [Route("aisp/accounts/{externalApiAccountId}/party")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PartiesResponse>> GetAsync(
        string? externalApiAccountId,
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required]
        Guid accountAccessConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Support pass-through of all query parameters
        string? queryString = null;
        if (!string.IsNullOrEmpty(HttpContext.Request.QueryString.Value))
        {
            queryString = HttpContext.Request.QueryString.Value;
        }

        // Operation
        PartiesResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Parties
            .ReadAsync(
                accountAccessConsentId,
                externalApiAccountId,
                modifiedBy,
                queryString,
                requestUrlWithoutQuery);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Read parties
    /// </summary>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("aisp/accounts/{externalApiAccountId}/parties")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Parties2Response>> Get2Async(
        string? externalApiAccountId,
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required]
        Guid accountAccessConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Support pass-through of all query parameters
        string queryString = HttpContext.Request.QueryString.Value ?? string.Empty;

        // Operation
        Parties2Response fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Parties2
            .ReadAsync(
                accountAccessConsentId,
                externalApiAccountId,
                modifiedBy,
                queryString,
                requestUrlWithoutQuery);

        return Ok(fluentResponse);
    }
}
