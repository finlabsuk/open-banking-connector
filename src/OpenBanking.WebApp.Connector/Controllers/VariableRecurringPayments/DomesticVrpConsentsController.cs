// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.VariableRecurringPayments;

[ApiController]
[ApiExplorerSettings(GroupName = "vrp")]
[Tags("Domestic VRP Consents")]
[Route("vrp/domestic-vrp-consents")]
public class DomesticVrpConsentsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public DomesticVrpConsentsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Create domestic VRP consent
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticVrpConsentCreateResponse>> PostAsync(
        [FromBody]
        DomesticVrpConsentRequest request)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        DomesticVrpConsentCreateResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .CreateAsync(request, requestUrlWithoutQuery);

        return CreatedAtAction(
            nameof(GetAsync),
            new { domesticVrpConsentId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read domestic VRP consent
    /// </summary>
    /// <param name="domesticVrpConsentId">ID of DomesticVrpConsent</param>
    /// <param name="modifiedBy"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <returns></returns>
    [HttpGet("{domesticVrpConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticVrpConsentCreateResponse>> GetAsync(
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "x-include-external-api-operation")]
        bool? includeExternalApiOperation)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Operation
        DomesticVrpConsentCreateResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .ReadAsync(domesticVrpConsentId, modifiedBy, includeExternalApiOperation ?? true, requestUrlWithoutQuery);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete domestic VRP consent
    /// </summary>
    /// <param name="domesticVrpConsentId">ID of DomesticVrpConsent</param>
    /// <param name="modifiedBy"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <returns></returns>
    [HttpDelete("{domesticVrpConsentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "x-include-external-api-operation")]
        bool? includeExternalApiOperation)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .DeleteAsync(domesticVrpConsentId, modifiedBy, includeExternalApiOperation ?? true);

        return Ok(fluentResponse);
    }
}
