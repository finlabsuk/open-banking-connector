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
[Route("config/variable-recurring-payments-apis")]
public class VariableRecurringPaymentsApisController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public VariableRecurringPaymentsApisController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create VariableRecurringPaymentsApi
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VariableRecurringPaymentsApiResponse))]
    public async Task<IActionResult> PostAsync([FromBody] VariableRecurringPaymentsApiRequest request)
    {
        // Operation
        VariableRecurringPaymentsApiResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .VariableRecurringPaymentsApis
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { variableRecurringPaymentsApiId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read VariableRecurringPaymentsApi
    /// </summary>
    /// <param name="variableRecurringPaymentsApiId"></param>
    /// <returns></returns>
    [HttpGet("{variableRecurringPaymentsApiId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VariableRecurringPaymentsApiResponse))]
    public async Task<IActionResult> GetAsync(Guid variableRecurringPaymentsApiId)
    {
        // Operation
        VariableRecurringPaymentsApiResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .VariableRecurringPaymentsApis
            .ReadLocalAsync(variableRecurringPaymentsApiId);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete VariableRecurringPaymentsApi
    /// </summary>
    /// <param name="variableRecurringPaymentsApiId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{variableRecurringPaymentsApiId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjectDeleteResponse))]
    public async Task<IActionResult> DeleteAsync(
        Guid variableRecurringPaymentsApiId,
        [FromHeader]
        string? modifiedBy)
    {
        // Operation
        ObjectDeleteResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .VariableRecurringPaymentsApis
            .DeleteLocalAsync(variableRecurringPaymentsApiId, modifiedBy);

        return Ok(fluentResponse);
    }
}
