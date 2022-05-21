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
    ///     Create an AccountAccessConsent
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/account-access-consents")]
    [HttpPost]
    [ProducesResponseType(
        typeof(AccountAccessConsentReadResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] AccountAccessConsent request)
    {
        IFluentResponse<AccountAccessConsentReadResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .CreateAsync(request);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentReadResponse> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<AccountAccessConsentReadResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<AccountAccessConsentReadResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    ///     Read an AccountAccessConsent
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/account-access-consents/{accountAccessConsentId}")]
    [HttpGet]
    [ProducesResponseType(
        typeof(AccountAccessConsentReadResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(Guid accountAccessConsentId,
        [FromHeader]
        string? modifiedBy)
    {
        // Operation
        IFluentResponse<AccountAccessConsentReadResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .ReadAsync(accountAccessConsentId,modifiedBy);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentReadResponse> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<AccountAccessConsentReadResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<AccountAccessConsentReadResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    ///     Delete an AccountAccessConsent
    /// </summary>
    /// <param name="accountAccessConsentId">ID of AccountAccessConsent</param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/account-access-consents/{accountAccessConsentId}")]
    [HttpDelete]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(Guid accountAccessConsentId,
        [FromHeader]
        string? modifiedBy)
    {
        // Operation
        IFluentResponse fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .DeleteAsync(accountAccessConsentId, modifiedBy);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
