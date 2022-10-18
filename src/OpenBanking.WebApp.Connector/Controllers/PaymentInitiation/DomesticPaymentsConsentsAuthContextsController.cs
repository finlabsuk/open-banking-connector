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
[Tags("Domestic Payment Consent Auth Contexts")]
[Route("pisp/domestic-payment-consent-auth-contexts")]
public class DomesticPaymentsConsentsAuthContextsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public DomesticPaymentsConsentsAuthContextsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create DomesticPaymentConsent AuthContext
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(
        StatusCodes.Status201Created,
        Type = typeof(DomesticPaymentConsentAuthContextCreateResponse))]
    public async Task<IActionResult> PostAsync([FromBody] DomesticPaymentConsentAuthContext request)
    {
        DomesticPaymentConsentAuthContextCreateResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .AuthContexts
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { domesticPaymentConsentAuthContextId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read DomesticPaymentConsent AuthContext
    /// </summary>
    /// <param name="domesticPaymentConsentAuthContextId">ID of DomesticPaymentConsent AuthContext</param>
    /// <returns></returns>
    [HttpGet("{domesticPaymentConsentAuthContextId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DomesticPaymentConsentAuthContextReadResponse))]
    public async Task<IActionResult> GetAsync(Guid domesticPaymentConsentAuthContextId)
    {
        // Operation
        DomesticPaymentConsentAuthContextReadResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .AuthContexts
            .ReadLocalAsync(domesticPaymentConsentAuthContextId);

        return Ok(fluentResponse);
    }
}
