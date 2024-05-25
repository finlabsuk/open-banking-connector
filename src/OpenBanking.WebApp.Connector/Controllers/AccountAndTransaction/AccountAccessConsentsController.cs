// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Account Access Consents")]
[Route("aisp/account-access-consents")]
public class AccountAccessConsentsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public AccountAccessConsentsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Create account access consent
    /// </summary>
    /// <param name="request"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        AccountAccessConsentCreateResponse>> PostAsync(
        [FromBody]
        AccountAccessConsentRequest request,
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

        AccountAccessConsentCreateResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .CreateAsync(request, requestUrlWithoutQuery, extraHeaders);

        return CreatedAtAction(
            nameof(GetAsync),
            new { accountAccessConsentId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read account access consent
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent</param>
    /// <param name="excludeExternalApiOperation"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{accountAccessConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        AccountAccessConsentCreateResponse>> GetAsync(
        Guid accountAccessConsentId,
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
        AccountAccessConsentCreateResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .ReadAsync(
                new ConsentReadParams
                {
                    Id = accountAccessConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExcludeExternalApiOperation = excludeExternalApiOperation ?? false
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete account access consent
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent</param>
    /// <param name="excludeExternalApiOperation"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpDelete("{accountAccessConsentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        BaseResponse>> DeleteAsync(
        Guid accountAccessConsentId,
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
            .AccountAndTransaction
            .AccountAccessConsents
            .DeleteAsync(
                new ConsentDeleteParams
                {
                    ExtraHeaders = extraHeaders,
                    ExcludeExternalApiOperation = excludeExternalApiOperation ?? false,
                    Id = accountAccessConsentId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }
}
