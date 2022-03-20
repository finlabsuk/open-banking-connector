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

/// <summary>
///     HTTP response object used when reading Accounts objects. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class AccountsHttpResponse : HttpResponse<AccountsResponse>
{
    public AccountsHttpResponse(HttpResponseMessages? messages, AccountsResponse? data) :
        base(messages, data) { }
}

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
public class AccountsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public AccountsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Get Accounts
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/accounts")]
    [HttpGet]
    [ProducesResponseType(
        typeof(AccountsHttpResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(AccountsHttpResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(AccountsHttpResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] string accountAccessConsentId)
    {
        // Operation
        IFluentResponse<AccountsResponse> fluentResponse = null!;

        // HTTP response
        var httpResponse = (AccountsHttpResponse) fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<AccountsResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<AccountsResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<AccountsResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
