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

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public TransactionsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Read Transactions
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="externalApiStatementId">External (bank) API ID of Statement</param>
    /// <param name="fromBookingDateTime">
    ///     Start date and time for filtering of the Transaction records on the
    ///     Transaction/BookingDateTime field
    /// </param>
    /// <param name="toBookingDateTime">
    ///     End date and time for filtering of the Transaction records on the
    ///     Transaction/BookingDateTime field
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/transactions")]
    [Route("aisp/accounts/{externalApiAccountId}/transactions")]
    [Route("aisp/accounts/{externalApiAccountId}/statements/{externalApiStatementId}/transactions")]
    [HttpGet]
    [ProducesResponseType(
        typeof(TransactionsResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] Guid accountAccessConsentId,
        [FromQuery]
        string? fromBookingDateTime,
        [FromQuery]
        string? toBookingDateTime,
        string? externalApiAccountId,
        string? externalApiStatementId)
    {
        // Operation
        IFluentResponse<TransactionsResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Transactions
            .ReadAsync(
                accountAccessConsentId,
                externalApiAccountId,
                externalApiStatementId,
                fromBookingDateTime,
                toBookingDateTime);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<TransactionsResponse> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<TransactionsResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<TransactionsResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
