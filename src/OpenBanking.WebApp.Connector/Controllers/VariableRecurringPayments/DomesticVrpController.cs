// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.VariableRecurringPayments;

[ApiController]
[ApiExplorerSettings(GroupName = "vrp")]
[Route("vrp/domestic-vrps")]
[Tags("Domestic VRPs")]
public class DomesticVrpController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public DomesticVrpController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Create domestic VRP
    /// </summary>
    /// <param name="request"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticVrpResponse>> PostAsync(
        [FromBody]
        DomesticVrpRequest request,
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

        DomesticVrpResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrps
            .CreateAsync(
                request,
                new ConsentExternalCreateParams
                {
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery
                });

        return CreatedAtAction(
            nameof(GetAsync),
            new { externalApiId = fluentResponse.ExternalApiResponse.Data.DomesticVRPId },
            fluentResponse);
    }

    /// <summary>
    ///     Read domestic VRP
    /// </summary>
    /// <param name="externalApiId">External (bank) API ID of Domestic VRP</param>
    /// <param name="bankRegistrationId"></param>
    /// <param name="useV4ExternalApi"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{externalApiId}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticVrpResponse>> GetAsync(
        string externalApiId,
        [FromHeader(Name = "x-obc-bank-registration-id")]
        Guid bankRegistrationId,
        [FromHeader(Name = "x-obc-use-v4-external-api")]
        bool? useV4ExternalApi,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        if (bankRegistrationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Required header x-obc-bank-registration-id either set to empty value or not provided.");
        }

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
        DomesticVrpResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrps
            .ReadAsync(
                new ExternalEntityReadParams
                {
                    BankRegistrationId = bankRegistrationId,
                    UseV4ExternalApi = useV4ExternalApi,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExternalApiId = externalApiId
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Read domestic VRP payment details
    /// </summary>
    /// <param name="externalApiId">External (bank) API ID of Domestic VRP</param>
    /// <param name="bankRegistrationId"></param>
    /// <param name="useV4ExternalApi"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{externalApiId}/payment-details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticVrpPaymentDetailsResponse>> GetPaymentDetailsAsync(
        string externalApiId,
        [FromHeader(Name = "x-obc-bank-registration-id")]
        Guid bankRegistrationId,
        [FromHeader(Name = "x-obc-use-v4-external-api")]
        bool? useV4ExternalApi,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        if (bankRegistrationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Required header x-obc-bank-registration-id either set to empty value or not provided.");
        }

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
        DomesticVrpPaymentDetailsResponse fluentResponse = await _requestBuilder
            .VariableRecurringPayments
            .DomesticVrps
            .ReadPaymentDetailsAsync(
                new ExternalEntityReadParams
                {
                    BankRegistrationId = bankRegistrationId,
                    UseV4ExternalApi = useV4ExternalApi,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExternalApiId = externalApiId
                });

        return Ok(fluentResponse);
    }
}
