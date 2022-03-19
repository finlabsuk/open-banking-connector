// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Account Access Consents")]
public class AccountAccessConsentsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public AccountAccessConsentsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create an AccountAccessConsent object
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/account-access-consents")]
    [HttpPost]
    [ProducesResponseType(
        typeof(HttpResponse<AccountAccessConsentResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponse<AccountAccessConsentResponse>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<AccountAccessConsentResponse>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] AccountAccessConsent request)
    {
        IFluentResponse<AccountAccessConsentResponse> fluentResponse = null!;

        // HTTP response
        HttpResponse<AccountAccessConsentResponse> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentResponse> _ => StatusCodes.Status201Created,
            FluentBadRequestErrorResponse<AccountAccessConsentResponse> _ => StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<AccountAccessConsentResponse> _ => StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }

    /// <summary>
    ///     Read an AccountAccessConsent object
    /// </summary>
    /// <param name="Id">ID of AccountAccessConsent object</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/account-access-consents/{Id}")]
    [HttpGet]
    [ProducesResponseType(
        typeof(HttpResponse<AccountAccessConsentResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponse<AccountAccessConsentResponse>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<AccountAccessConsentResponse>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(string Id)
    {
        // Operation
        IFluentResponse<AccountAccessConsentResponse> fluentResponse = null!;

        // HTTP response
        HttpResponse<AccountAccessConsentResponse> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<AccountAccessConsentResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<AccountAccessConsentResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
