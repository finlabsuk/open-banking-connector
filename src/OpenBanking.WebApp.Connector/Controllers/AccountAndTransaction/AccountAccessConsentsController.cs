// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Account Access Consents")]
[Route("aisp/account-access-consents")]
public class AccountAccessConsentsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public AccountAccessConsentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create AccountAccessConsent
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountAccessConsentResponse))]
    public async Task<IActionResult> PostAsync([FromBody] AccountAccessConsent request)
    {
        AccountAccessConsentResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .CreateAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { accountAccessConsentId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read AccountAccessConsent
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{accountAccessConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountAccessConsentResponse))]
    public async Task<IActionResult> GetAsync(
        Guid accountAccessConsentId,
        [FromHeader]
        string? modifiedBy)
    {
        // Operation
        AccountAccessConsentResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .ReadAsync(accountAccessConsentId, modifiedBy);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete AccountAccessConsent
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent</param>
    /// <param name="modifiedBy"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <returns></returns>
    [HttpDelete("{accountAccessConsentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjectDeleteResponse))]
    public async Task<IActionResult> DeleteAsync(
        Guid accountAccessConsentId,
        [FromHeader]
        string? modifiedBy,
        [FromHeader]
        bool? includeExternalApiOperation)
    {
        // Operation
        ObjectDeleteResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .DeleteAsync(accountAccessConsentId, modifiedBy, includeExternalApiOperation ?? true);

        return Ok(fluentResponse);
    }
}
