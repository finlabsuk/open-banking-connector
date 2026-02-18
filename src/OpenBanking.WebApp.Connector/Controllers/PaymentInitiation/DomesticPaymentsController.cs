// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
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
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public DomesticPaymentsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Create domestic payment
    /// </summary>
    /// <param name="request"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DomesticPaymentResponse>> PostAsync(
        [FromBody]
        DomesticPaymentRequest request,
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

        DomesticPaymentResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPayments
            .CreateAsync(
                request,
                new ConsentExternalCreateParams
                {
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery
                });

        return CreatedAtAction(
            nameof(GetAsync),
            new { externalApiId = fluentResponse.ExternalApiResponse.Data.DomesticPaymentId },
            fluentResponse);
    }


    /// <summary>
    ///     Read domestic payment
    /// </summary>
    /// <param name="externalApiId">External (bank) API ID of Domestic Payment</param>
    /// <param name="bankRegistrationId"></param>
    /// <param name="useV4ExternalApi"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{externalApiId}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticPaymentResponse>> GetAsync(
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
        DomesticPaymentResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPayments
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
    ///     Read domestic payment payment details
    /// </summary>
    /// <param name="externalApiId">External (bank) API ID of Domestic Payment</param>
    /// <param name="bankRegistrationId"></param>
    /// <param name="useV4ExternalApi"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{externalApiId}/payment-details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DomesticPaymentPaymentDetailsResponse>> GetPaymentDetailsAsync(
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
        DomesticPaymentPaymentDetailsResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPayments
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
