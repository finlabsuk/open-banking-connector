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
[Route("pisp/domestic-payments")]
[Tags("Domestic Payments")]
public class DomesticPaymentsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public DomesticPaymentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create domestic payment
    /// </summary>
    /// <param name="domesticPaymentConsentId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticPaymentResponse>> PostAsync(
        [FromHeader(Name = "x-obc-domestic-payment-consent-id")]
        Guid domesticPaymentConsentId,
        [FromBody]
        DomesticPaymentRequest request)
    {
        DomesticPaymentResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPayments
            .CreateAsync(request, domesticPaymentConsentId);

        return CreatedAtAction(
            nameof(GetAsync),
            new { externalApiId = fluentResponse.ExternalApiResponse.Data.DomesticPaymentId },
            fluentResponse);
    }


    /// <summary>
    ///     Read domestic payment
    /// </summary>
    /// <param name="externalApiId">External (bank) API ID of Domestic Payment</param>
    /// <param name="domesticPaymentConsentId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{externalApiId}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticPaymentResponse>> GetAsync(
        string externalApiId,
        [FromHeader(Name = "x-obc-domestic-payment-consent-id")]
        Guid domesticPaymentConsentId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        DomesticPaymentResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPayments
            .ReadAsync(externalApiId, domesticPaymentConsentId, modifiedBy);

        return Ok(fluentResponse);
    }
}
