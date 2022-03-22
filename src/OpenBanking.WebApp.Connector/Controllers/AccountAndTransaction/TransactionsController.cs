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
///     HTTP response object used when reading Transactions objects. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class TransactionsHttpResponse : HttpResponse<TransactionsResponse>
{
    public TransactionsHttpResponse(HttpResponseMessages? messages, TransactionsResponse? data) :
        base(messages, data) { }
}

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
public class TransactionsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public TransactionsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Get Transactions
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="externalAccountId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/transactions")]
    [Route("aisp/transactions/{ExternalAccountId}")]
    [HttpGet]
    [ProducesResponseType(
        typeof(TransactionsHttpResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(TransactionsHttpResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(TransactionsHttpResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] Guid accountAccessConsentId,
        string? externalAccountId)
    {
        // Operation
        IFluentResponse<TransactionsResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Transactions
            .ReadAsync(accountAccessConsentId, externalAccountId);

        // HTTP response
        HttpResponse<TransactionsResponse> httpResponseTmp = fluentResponse.ToHttpResponse();
        var httpResponse = new TransactionsHttpResponse(httpResponseTmp.Messages, httpResponseTmp.Data);
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<TransactionsResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<TransactionsResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<TransactionsResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
