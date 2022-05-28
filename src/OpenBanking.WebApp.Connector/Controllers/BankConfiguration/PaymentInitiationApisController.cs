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
[Route("config/payment-initiation-apis")]
public class PaymentInitiationApisController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public PaymentInitiationApisController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create PaymentInitiationApi
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PaymentInitiationApiResponse))]
    public async Task<IActionResult> PostAsync([FromBody] PaymentInitiationApiRequest request)
    {
        // Operation
        PaymentInitiationApiResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .PaymentInitiationApis
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { paymentInitiationApiId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read PaymentInitiationApi
    /// </summary>
    /// <param name="paymentInitiationApiId"></param>
    /// <returns></returns>
    [HttpGet("{paymentInitiationApiId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentInitiationApiResponse))]
    public async Task<IActionResult> GetAsync(Guid paymentInitiationApiId)
    {
        // Operation
        PaymentInitiationApiResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .PaymentInitiationApis
            .ReadLocalAsync(paymentInitiationApiId);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Read all PaymentInitiationApi objects (temporary endpoint)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<PaymentInitiationApiResponse>))]
    public async Task<IActionResult> GetAsync()
    {
        // Operation
        IQueryable<PaymentInitiationApiResponse> fluentResponse = await _requestBuilder
            .BankConfiguration
            .PaymentInitiationApis
            .ReadLocalAsync(query => true);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete PaymentInitiationApi
    /// </summary>
    /// <param name="bankId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{paymentInitiationApiId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjectDeleteResponse))]
    public async Task<IActionResult> DeleteAsync(
        Guid bankId,
        [FromHeader]
        string? modifiedBy)
    {
        // Operation
        ObjectDeleteResponse fluentResponse = await _requestBuilder
            .BankConfiguration
            .PaymentInitiationApis
            .DeleteLocalAsync(bankId, modifiedBy);

        return Ok(fluentResponse);
    }
}
