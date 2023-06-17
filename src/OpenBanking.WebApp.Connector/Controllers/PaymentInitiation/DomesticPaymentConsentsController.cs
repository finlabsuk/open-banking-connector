// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.PaymentInitiation;

[ApiController]
[ApiExplorerSettings(GroupName = "pisp")]
[Tags("Domestic Payment Consents")]
[Route("pisp/domestic-payment-consents")]
public class DomesticPaymentConsentsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public DomesticPaymentConsentsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Create domestic payment consent
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        DomesticPaymentConsentCreateResponse>> PostAsync([FromBody] DomesticPaymentConsentRequest request)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        DomesticPaymentConsentCreateResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .CreateAsync(request, requestUrlWithoutQuery);

        return CreatedAtAction(
            nameof(GetAsync),
            new { domesticPaymentConsentId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read domestic payment consent
    /// </summary>
    /// <param name="domesticPaymentConsentId">ID of DomesticPaymentConsent</param>
    /// <param name="modifiedBy"></param>
    /// <param name="includeExternalApiOperation"></param>
    /// <returns></returns>
    [HttpGet("{domesticPaymentConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        DomesticPaymentConsentCreateResponse>> GetAsync(
        Guid domesticPaymentConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy,
        [FromHeader(Name = "x-include-external-api-operation")]
        bool? includeExternalApiOperation)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Operation
        DomesticPaymentConsentCreateResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .ReadAsync(
                domesticPaymentConsentId,
                modifiedBy,
                includeExternalApiOperation ?? true,
                requestUrlWithoutQuery);

        return Ok(fluentResponse);
    }
}
