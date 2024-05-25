// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
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
    ///     Read transactions
    /// </summary>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="externalApiStatementId">External (bank) API ID of Statement</param>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="fromBookingDateTime">
    ///     Start date and time for filtering of the Transaction records on the
    ///     Transaction/BookingDateTime field
    /// </param>
    /// <param name="toBookingDateTime">
    ///     End date and time for filtering of the Transaction records on the
    ///     Transaction/BookingDateTime field
    /// </param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("aisp/transactions")]
    [Route("aisp/accounts/{externalApiAccountId}/transactions")]
    [Route("aisp/accounts/{externalApiAccountId}/statements/{externalApiStatementId}/transactions")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TransactionsResponse>> GetAsync(
        string? externalApiAccountId,
        string? externalApiStatementId,
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required]
        Guid accountAccessConsentId,
        [FromQuery]
        string? fromBookingDateTime,
        [FromQuery]
        string? toBookingDateTime,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
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

        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        // Operation
        TransactionsResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Transactions
            .ReadAsync(
                new TransactionsReadParams
                {
                    ExternalApiStatementId = externalApiStatementId,
                    ConsentId = accountAccessConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExternalApiAccountId = externalApiAccountId,
                    QueryString = queryString
                });

        return Ok(fluentResponse);
    }
}
