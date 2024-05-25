// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.VariableRecurringPayments;

[ApiController]
[ApiExplorerSettings(GroupName = "vrp")]
[Tags("Domestic VRP Consent Auth Contexts")]
[Route("vrp/domestic-vrp-consent-auth-contexts")]
public class DomesticVrpsConsentsAuthContextsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public DomesticVrpsConsentsAuthContextsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create domestic VRP consent auth context (time-sensitive auth session) and return auth URL
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        DomesticVrpConsentAuthContextCreateResponse>> PostAsync(
        [FromBody]
        DomesticVrpConsentAuthContext request)
    {
        DomesticVrpConsentAuthContextCreateResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .AuthContexts
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { domesticVrpConsentAuthContextId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read domestic VRP consent auth context
    /// </summary>
    /// <param name="domesticVrpConsentAuthContextId">ID of DomesticVrpConsent AuthContext</param>
    /// <returns></returns>
    [HttpGet("{domesticVrpConsentAuthContextId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        DomesticVrpConsentAuthContextReadResponse>> GetAsync(Guid domesticVrpConsentAuthContextId)
    {
        // Operation
        DomesticVrpConsentAuthContextReadResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .AuthContexts
            .ReadLocalAsync(
                new LocalReadParams
                {
                    Id = domesticVrpConsentAuthContextId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }
}
