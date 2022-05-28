// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.BankConfiguration;

[ApiController]
[ApiExplorerSettings(GroupName = "config")]
[Route("config/account-and-transaction-apis")]
public class AccountAndTransactionApisController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public AccountAndTransactionApisController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create AccountAndTransactionApi
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountAndTransactionApiResponse))]
    public async Task<IActionResult> PostAsync([FromBody] AccountAndTransactionApiRequest request)
    {
        // Operation
        AccountAndTransactionApiResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .AccountAndTransactionApis
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { accountAndTransactionApiId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read AccountAndTransactionApi
    /// </summary>
    /// <param name="accountAndTransactionApiId"></param>
    /// <returns></returns>
    [HttpGet("{accountAndTransactionApiId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountAndTransactionApiResponse))]
    public async Task<IActionResult> GetAsync(Guid accountAndTransactionApiId)
    {
        // Operation
        AccountAndTransactionApiResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .AccountAndTransactionApis
            .ReadLocalAsync(accountAndTransactionApiId);

        return Ok(fluentResponse);
    }


    /// <summary>
    ///     Read all AccountAndTransactionApi objects (temporary endpoint)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(IList<AccountAndTransactionApiResponse>))]
    public async Task<IActionResult> GetAsync()
    {
        // Operation
        IQueryable<AccountAndTransactionApiResponse> fluentResponse = await _requestBuilder
            .BankConfiguration
            .AccountAndTransactionApis
            .ReadLocalAsync(query => true);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete AccountAndTransactionApi
    /// </summary>
    /// <param name="bankId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{accountAndTransactionApiId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjectDeleteResponse))]
    public async Task<IActionResult> DeleteAsync(
        Guid bankId,
        [FromHeader]
        string? modifiedBy)
    {
        // Operation
        ObjectDeleteResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .AccountAndTransactionApis
            .DeleteLocalAsync(bankId, modifiedBy);

        return Ok(fluentResponse);
    }
}
