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
[Route("config/banks")]
public class BanksController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public BanksController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create Bank object
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BankResponse))]
    public async Task<IActionResult> PostAsync([FromBody] Bank request)
    {
        // Operation
        BankResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .Banks
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { bankId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read Bank object
    /// </summary>
    /// <param name="bankId"></param>
    /// <returns></returns>
    [HttpGet("{bankId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankResponse))]
    public async Task<IActionResult> GetAsync(Guid bankId)
    {
        // Operation
        BankResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .Banks
            .ReadLocalAsync(bankId);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete Bank object
    /// </summary>
    /// <param name="bankId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{bankId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjectDeleteResponse))]
    public async Task<IActionResult> DeleteAsync(
        Guid bankId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        ObjectDeleteResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .Banks
            .DeleteLocalAsync(bankId, modifiedBy);

        return Ok(fluentResponse);
    }
}
