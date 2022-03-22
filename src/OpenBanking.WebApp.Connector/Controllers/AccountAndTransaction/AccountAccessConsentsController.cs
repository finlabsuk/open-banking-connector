// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;
using HttpResponse = FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response.HttpResponse;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

/// <summary>
///     HTTP response object used when creating or reading an AccountAccessConsent object. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class AccountAccessConsentHttpReadResponse : HttpResponse<AccountAccessConsentReadResponse>
{
    public AccountAccessConsentHttpReadResponse(
        HttpResponseMessages? messages,
        AccountAccessConsentReadResponse? data) :
        base(messages, data) { }
}

/// <summary>
///     HTTP response object used when local-reading an AccountAccessConsent object. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class AccountAccessConsentHttpReadLocalResponse : HttpResponse<IList<AccountAccessConsentReadLocalResponse>>
{
    public AccountAccessConsentHttpReadLocalResponse(
        HttpResponseMessages? messages,
        IList<AccountAccessConsentReadLocalResponse>? data) :
        base(messages, data) { }
}

/// <summary>
///     HTTP response object used when deleting an AccountAccessConsent object. Includes messages from
///     Open Banking Connector.
/// </summary>
public class AccountAccessConsentHttpDeleteResponse : HttpResponse
{
    public AccountAccessConsentHttpDeleteResponse(HttpResponseMessages? messages) : base(messages) { }
}

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
        typeof(AccountAccessConsentHttpReadResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(AccountAccessConsentHttpReadResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(AccountAccessConsentHttpReadResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] AccountAccessConsent request)
    {
        IFluentResponse<AccountAccessConsentReadResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .CreateAsync(request);

        // HTTP response
        var httpResponseTmp = fluentResponse.ToHttpResponse();
        AccountAccessConsentHttpReadResponse httpResponse =
            new AccountAccessConsentHttpReadResponse(httpResponseTmp.Messages, httpResponseTmp.Data);
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentReadResponse> _ => StatusCodes.Status201Created,
            FluentBadRequestErrorResponse<AccountAccessConsentReadResponse> _ => StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<AccountAccessConsentReadResponse> _ => StatusCodes.Status500InternalServerError,
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
        typeof(AccountAccessConsentHttpReadResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(AccountAccessConsentHttpReadResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(AccountAccessConsentHttpReadResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(Guid Id)
    {
        // Operation
        IFluentResponse<AccountAccessConsentReadResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .ReadAsync(Id);

        // HTTP response
        var httpResponse = (AccountAccessConsentHttpReadResponse) fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentReadResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<AccountAccessConsentReadResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<AccountAccessConsentReadResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }

    /// <summary>
    ///     Soft-delete an AccountAccessConsent object
    /// </summary>
    /// <param name="Id">ID of AccountAccessConsent object</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/account-access-consents/{Id}")]
    [HttpDelete]
    [ProducesResponseType(
        typeof(AccountAccessConsentHttpDeleteResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(AccountAccessConsentHttpDeleteResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(AccountAccessConsentHttpDeleteResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(Guid Id)
    {
        // Operation
        IFluentResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .DeleteLocalAsync(Id);

        // HTTP response
        var httpResponse = (AccountAccessConsentHttpDeleteResponse) fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentHttpDeleteResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<AccountAccessConsentHttpDeleteResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<AccountAccessConsentHttpDeleteResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
