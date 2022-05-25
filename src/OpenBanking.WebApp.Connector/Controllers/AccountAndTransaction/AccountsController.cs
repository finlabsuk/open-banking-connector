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
[Tags("Accounts")]
public class AccountsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public AccountsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Read Account(s)
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="modifiedBy"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/accounts")]
    [Route("aisp/accounts/{externalApiAccountId}")]
    [HttpGet]
    [ProducesResponseType(
        typeof(AccountsResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        string? externalApiAccountId,
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required]
        Guid accountAccessConsentId,
        [FromHeader]
        string? modifiedBy,
        [FromQuery]
        string? page)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Operation
        IFluentResponse<AccountsResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Accounts
            .ReadAsync(
                accountAccessConsentId,
                externalApiAccountId,
                null,
                null,
                null,
                page ?? string.Empty,
                modifiedBy,
                requestUrlWithoutQuery);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<AccountsResponse> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<AccountsResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<AccountsResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
