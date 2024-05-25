// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Account Access Consent Auth Contexts")]
[Route("aisp/account-access-consent-auth-contexts")]
public class AccountAccessConsentsAuthContextsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public AccountAccessConsentsAuthContextsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create account access consent auth context (time-sensitive auth session) and return auth URL
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        AccountAccessConsentAuthContextCreateResponse>> PostAsync([FromBody] AccountAccessConsentAuthContext request)
    {
        AccountAccessConsentAuthContextCreateResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .AuthContexts
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { accountAccessConsentAuthContextId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read account access consent auth context
    /// </summary>
    /// <param name="accountAccessConsentAuthContextId">ID of AccountAccessConsent AuthContext</param>
    /// <returns></returns>
    [HttpGet("{accountAccessConsentAuthContextId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        AccountAccessConsentAuthContextReadResponse>> GetAsync(Guid accountAccessConsentAuthContextId)
    {
        // Operation
        AccountAccessConsentAuthContextReadResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .AuthContexts
            .ReadLocalAsync(
                new LocalReadParams
                {
                    Id = accountAccessConsentAuthContextId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }
}
