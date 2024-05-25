// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Direct Debits")]
public class DirectDebitsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public DirectDebitsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Read direct debits
    /// </summary>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("aisp/direct-debits")]
    [Route("aisp/accounts/{externalApiAccountId}/direct-debits")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DirectDebitsResponse>> GetAsync(
        string? externalApiAccountId,
        [FromHeader(Name = "x-obc-account-access-consent-id")]
        Guid accountAccessConsentId,
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
        DirectDebitsResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .DirectDebits
            .ReadAsync(
                new AccountAccessConsentExternalReadParams
                {
                    ExternalApiAccountId = externalApiAccountId,
                    QueryString = queryString,
                    ConsentId = accountAccessConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery
                });

        return Ok(fluentResponse);
    }
}
