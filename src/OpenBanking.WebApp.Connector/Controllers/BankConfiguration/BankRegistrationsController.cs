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
[ApiExplorerSettings(GroupName = "manage")]
[Route("manage/bank-registrations")]
[Tags("Bank Registrations")]
public class BankRegistrationsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public BankRegistrationsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create bank registration
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        BankRegistrationResponse>> PostAsync([FromBody] BankRegistration request)
    {
        // Operation
        BankRegistrationResponse fluentResponse = await _requestBuilder
            .Management
            .BankRegistrations
            .CreateAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { bankRegistrationId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read bank registration
    /// </summary>
    /// <param name="bankRegistrationId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{bankRegistrationId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        BankRegistrationResponse>> GetAsync(
        Guid bankRegistrationId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        BankRegistrationResponse fluentResponse = await _requestBuilder
            .Management
            .BankRegistrations
            .ReadAsync(
                bankRegistrationId,
                modifiedBy);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete bank registration
    /// </summary>
    /// <param name="bankRegistrationId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{bankRegistrationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid bankRegistrationId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .Management
            .BankRegistrations
            .DeleteAsync(
                bankRegistrationId,
                modifiedBy);

        return Ok(fluentResponse);
    }
}
