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
[Route("config/bank-registrations")]
public class BankRegistrationsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public BankRegistrationsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create BankRegistration
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BankRegistrationResponse))]
    public async Task<IActionResult> PostAsync([FromBody] BankRegistration request)
    {
        // Operation
        BankRegistrationResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .BankRegistrations
            .CreateAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { bankRegistrationId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read BankRegistration
    /// </summary>
    /// <param name="bankRegistrationId"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <returns></returns>
    [HttpGet("{bankRegistrationId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankRegistrationResponse))]
    public async Task<IActionResult> GetAsync(
        Guid bankRegistrationId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "x-obc-include-external-api-operation")]
        bool? includeExternalApiOperation)
    {
        // Operation
        BankRegistrationResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .BankRegistrations
            .ReadAsync(
                bankRegistrationId,
                modifiedBy,
                includeExternalApiOperation);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete BankRegistration
    /// </summary>
    /// <param name="bankRegistrationId"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <returns></returns>
    [HttpDelete("{bankRegistrationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjectDeleteResponse))]
    public async Task<IActionResult> DeleteAsync(
        Guid bankRegistrationId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "x-obc-include-external-api-operation")]
        bool? includeExternalApiOperation)
    {
        // Operation
        ObjectDeleteResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .BankRegistrations
            .DeleteAsync(
                bankRegistrationId,
                modifiedBy,
                includeExternalApiOperation);

        return Ok(fluentResponse);
    }
}
