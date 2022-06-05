// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Balances")]
public class BalancesController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public BalancesController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }


    /// <summary>
    ///     Read Balances
    /// </summary>
    /// <param name="externalApiAccountId">External (bank) API ID of Account</param>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("aisp/balances")]
    [Route("aisp/accounts/{externalApiAccountId}/balances")]
    [HttpGet]
    [ProducesResponseType(typeof(BalancesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        string? externalApiAccountId,
        [FromHeader(Name = "x-obc-account-access-consent-id")]
        Guid accountAccessConsentId,
        [FromHeader]
        string? modifiedBy)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Operation
        BalancesResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .Balances
            .ReadAsync(
                accountAccessConsentId,
                externalApiAccountId,
                modifiedBy: modifiedBy,
                requestUrlWithoutQuery: requestUrlWithoutQuery);

        return Ok(fluentResponse);
    }
}
