// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.VariableRecurringPayments;

[ApiController]
[ApiExplorerSettings(GroupName = "vrp")]
[Route("vrp/domestic-vrps")]
[Tags("Domestic VRPs")]
public class DomesticVrpController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public DomesticVrpController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create domestic VRP
    /// </summary>
    /// <param name="domesticVrpConsentId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticVrpResponse>> PostAsync(
        [FromHeader(Name = "x-obc-domestic-vrp-consent-id")] Guid domesticVrpConsentId,
        [FromBody]
        DomesticVrpRequest request)
    {
        DomesticVrpResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrps
            .CreateAsync(request, domesticVrpConsentId);

        return CreatedAtAction(
            nameof(GetAsync),
            new { externalApiId = fluentResponse.ExternalApiResponse.Data.DomesticVRPId },
            fluentResponse);
    }

    /// <summary>
    ///     Read domestic VRP
    /// </summary>
    /// <param name="externalApiId">External (bank) API ID of Domestic VRP</param>
    /// <param name="domesticVrpConsentId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{externalApiId}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticVrpResponse>> GetAsync(
        string externalApiId,
        [FromHeader(Name = "x-obc-domestic-vrp-consent-id")]
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        DomesticVrpResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrps
            .ReadAsync(externalApiId, domesticVrpConsentId, modifiedBy);

        return Ok(fluentResponse);
    }
}
