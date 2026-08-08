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
    /// <param name="xClientId">
    ///     Passed through to the bank. Only needed if the bank requires a client ID to return OB
    ///     v4.0.1 rate-limit headers.
    /// </param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticVrpConsentCreateResponse>> PostAsync(
        [FromBody]
        DomesticVrpConsentRequest request,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress,
        [FromHeader(Name = "x-client-id")]
        string? xClientId)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        var extraHeadersList = new List<HttpHeader>();
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress));
        }
        if (xClientId is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-client-id", xClientId));
        }
        IEnumerable<HttpHeader>? extraHeaders = extraHeadersList.Count > 0 ? extraHeadersList : null;

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
    ///     Update domestic VRP consent (migrate from v3 to v4)
    /// </summary>
    /// <param name="domesticVrpConsentId"></param>
    /// <param name="request"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <param name="xClientId">
    ///     Passed through to the bank. Only needed if the bank requires a client ID to return OB
    ///     v4.0.1 rate-limit headers.
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpPut("{domesticVrpConsentId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticVrpConsentCreateResponse>> PutAsync(
        Guid domesticVrpConsentId,
        [FromBody]
        DomesticVrpConsentRequest request,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress,
        [FromHeader(Name = "x-client-id")]
        string? xClientId)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        var extraHeadersList = new List<HttpHeader>();
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress));
        }
        if (xClientId is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-client-id", xClientId));
        }
        IEnumerable<HttpHeader>? extraHeaders = extraHeadersList.Count > 0 ? extraHeadersList : null;

        DomesticVrpConsentCreateResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .UpdateAsync(
                request,
                new ConsentBaseReadParams
                {
                    Id = domesticVrpConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Read domestic VRP consent
    /// </summary>
    /// <param name="domesticVrpConsentId">ID of DomesticVrpConsent</param>
    /// <param name="excludeExternalApiOperation"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <param name="xClientId">
    ///     Passed through to the bank. Only needed if the bank requires a client ID to return OB
    ///     v4.0.1 rate-limit headers.
    /// </param>
    /// <returns></returns>
    [HttpGet("{domesticVrpConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticVrpConsentCreateResponse>> GetAsync(
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-exclude-external-api-operation")]
        bool? excludeExternalApiOperation,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress,
        [FromHeader(Name = "x-client-id")]
        string? xClientId)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        var extraHeadersList = new List<HttpHeader>();
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress));
        }
        if (xClientId is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-client-id", xClientId));
        }
        IEnumerable<HttpHeader>? extraHeaders = extraHeadersList.Count > 0 ? extraHeadersList : null;

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
    /// <param name="xClientId">
    ///     Passed through to the bank. Only needed if the bank requires a client ID to return OB
    ///     v4.0.1 rate-limit headers.
    /// </param>
    /// <returns></returns>
    [HttpPost("{domesticVrpConsentId:guid}/funds-confirmation")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticVrpConsentFundsConfirmationResponse>> PostFundsConfirmationAsync(
        [FromBody]
        DomesticVrpConsentFundsConfirmationRequest request,
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress,
        [FromHeader(Name = "x-client-id")]
        string? xClientId)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        var extraHeadersList = new List<HttpHeader>();
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress));
        }
        if (xClientId is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-client-id", xClientId));
        }
        IEnumerable<HttpHeader>? extraHeaders = extraHeadersList.Count > 0 ? extraHeadersList : null;

        // Operation
        DomesticVrpConsentFundsConfirmationResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrpConsents
            .CreateFundsConfirmationAsync(
                new VrpConsentFundsConfirmationCreateParams
                {
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExtraHeaders = extraHeaders,
                    ConsentId = domesticVrpConsentId,
                    Request = request
                });

        return Created((string?) null, fluentResponse);
    }

    /// <summary>
    ///     Delete domestic VRP consent
    /// </summary>
    /// <param name="domesticVrpConsentId">ID of DomesticVrpConsent</param>
    /// <param name="excludeExternalApiOperation"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <param name="xClientId">
    ///     Passed through to the bank. Only needed if the bank requires a client ID to return OB
    ///     v4.0.1 rate-limit headers.
    /// </param>
    /// <returns></returns>
    [HttpDelete("{domesticVrpConsentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid domesticVrpConsentId,
        [FromHeader(Name = "x-obc-exclude-external-api-operation")]
        bool? excludeExternalApiOperation,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress,
        [FromHeader(Name = "x-client-id")]
        string? xClientId)
    {
        // Determine extra headers
        var extraHeadersList = new List<HttpHeader>();
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress));
        }
        if (xClientId is not null)
        {
            extraHeadersList.Add(new HttpHeader("x-client-id", xClientId));
        }
        IEnumerable<HttpHeader>? extraHeaders = extraHeadersList.Count > 0 ? extraHeadersList : null;

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
