﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Balances")]
public class BalancesController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public BalancesController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Read Balances
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/balances")]
    [Route("aisp/accounts/{externalApiAccountId}/balances")]
    [HttpGet]
    [ProducesResponseType(
        typeof(BalancesResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] Guid accountAccessConsentId,
        string? externalApiAccountId)
    {
        // Operation
        IFluentResponse<BalancesResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Balances
            .ReadAsync(accountAccessConsentId, externalApiAccountId);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<BalancesResponse> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<BalancesResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<BalancesResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
