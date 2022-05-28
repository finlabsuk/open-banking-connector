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
    private readonly IRequestBuilder _requestBuilder;

    public DomesticPaymentConsentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create DomesticPaymentConsent
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DomesticPaymentConsentResponse))]
    public async Task<IActionResult> PostAsync([FromBody] DomesticPaymentConsent request)
    {
        DomesticPaymentConsentResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .CreateAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { domesticPaymentConsentId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read DomesticPaymentConsent
    /// </summary>
    /// <param name="domesticPaymentConsentId">ID of DomesticPaymentConsent</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{domesticPaymentConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DomesticPaymentConsentResponse))]
    public async Task<IActionResult> GetAsync(
        Guid domesticPaymentConsentId,
        [FromHeader]
        string? modifiedBy)
    {
        // Operation
        DomesticPaymentConsentResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .ReadAsync(domesticPaymentConsentId, modifiedBy);

        return Ok(fluentResponse);
    }
}
