// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Monzo Pots")]
public class MonzoPotsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public MonzoPotsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Read Monzo pots
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent used for request (obtained when creating consent)</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("aisp/monzo-pots")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<MonzoPotsResponse>> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")]
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
        MonzoPotsResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .MonzoPots
            .ReadAsync(
                accountAccessConsentId,
                null,
                modifiedBy,
                queryString,
                requestUrlWithoutQuery);

        return Ok(fluentResponse);
    }
}
