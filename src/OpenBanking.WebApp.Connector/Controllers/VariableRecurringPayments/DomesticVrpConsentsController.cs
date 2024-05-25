// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
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
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticVrpConsentCreateResponse>> PostAsync(
        [FromBody]
        DomesticVrpConsentRequest request,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        DomesticVrpConsentCreateResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .CreateAsync(request, requestUrlWithoutQuery, extraHeaders);

        return CreatedAtAction(
            nameof(GetAsync),
            new { domesticVrpConsentId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read domestic VRP consent
    /// </summary>
    /// <param name="domesticVrpConsentId">ID of DomesticVrpConsent</param>
    /// <param name="excludeExternalApiOperation"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{domesticVrpConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticVrpConsentCreateResponse>> GetAsync(
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-exclude-external-api-operation")]
        bool? excludeExternalApiOperation,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        // Operation
        DomesticVrpConsentCreateResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .ReadAsync(
                new ConsentReadParams
                {
                    Id = domesticVrpConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExcludeExternalApiOperation = excludeExternalApiOperation ?? false
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Create domestic VRP consent funds confirmation
    /// </summary>
    /// <param name="request"></param>
    /// <param name="domesticVrpConsentId">ID of DomesticVrpConsent</param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpPost("{domesticVrpConsentId:guid}/funds-confirmation")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticVrpConsentFundsConfirmationResponse>> PostFundsConfirmationAsync(
        [FromBody]
        DomesticVrpConsentFundsConfirmationRequest request,
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        // Operation
        DomesticVrpConsentFundsConfirmationResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .CreateFundsConfirmationAsync(
                request,
                new VrpConsentFundsConfirmationCreateParams
                {
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExtraHeaders = extraHeaders,
                    Id = domesticVrpConsentId
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete domestic VRP consent
    /// </summary>
    /// <param name="domesticVrpConsentId">ID of DomesticVrpConsent</param>
    /// <param name="excludeExternalApiOperation"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpDelete("{domesticVrpConsentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-exclude-external-api-operation")]
        bool? excludeExternalApiOperation,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .DeleteAsync(
                new ConsentDeleteParams
                {
                    ExtraHeaders = extraHeaders,
                    ExcludeExternalApiOperation = excludeExternalApiOperation ?? false,
                    Id = domesticVrpConsentId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }
}
