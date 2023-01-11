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
[Tags("Transactions")]
public class TransactionsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public TransactionsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Read Transactions
    /// </summary>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="externalApiStatementId">External (bank) API ID of Statement</param>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="modifiedBy"></param>
    /// <param name="fromBookingDateTime">
    ///     Start date and time for filtering of the Transaction records on the
    ///     Transaction/BookingDateTime field
    /// </param>
    /// <param name="toBookingDateTime">
    ///     End date and time for filtering of the Transaction records on the
    ///     Transaction/BookingDateTime field
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("aisp/transactions")]
    [Route("aisp/accounts/{externalApiAccountId}/transactions")]
    [Route("aisp/accounts/{externalApiAccountId}/statements/{externalApiStatementId}/transactions")]
    [HttpGet]
    [ProducesResponseType(typeof(TransactionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        string? externalApiAccountId,
        string? externalApiStatementId,
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required]
        Guid accountAccessConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromQuery]
        string? fromBookingDateTime,
        [FromQuery]
        string? toBookingDateTime)
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
        TransactionsResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Transactions
            .ReadAsync(
                accountAccessConsentId,
                externalApiAccountId,
                externalApiStatementId,
                modifiedBy,
                queryString,
                requestUrlWithoutQuery);

        return Ok(fluentResponse);
    }
}
