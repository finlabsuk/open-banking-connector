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

/// <summary>
///     HTTP response object used when creating or reading an AccountAccessConsent object. Includes messages and data from
///     Open Banking Connector.
/// </summary>
public class
    AccountAccessConsentAuthContextsHttpReadResponse : HttpResponse<AccountAccessConsentAuthContextCreateLocalResponse>
{
    public AccountAccessConsentAuthContextsHttpReadResponse(
        HttpResponseMessages? messages,
        AccountAccessConsentAuthContextCreateLocalResponse? data) :
        base(messages, data) { }
}

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
[Tags("Account Access Consent Auth Contexts")]
public class AccountAccessConsentsAuthContextsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public AccountAccessConsentsAuthContextsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create an AccountAccessConsent AuthContext
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/account-access-consents/auth-contexts")]
    [HttpPost]
    [ProducesResponseType(
        typeof(AccountAccessConsentAuthContextsHttpReadResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(AccountAccessConsentAuthContextsHttpReadResponse),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(AccountAccessConsentAuthContextsHttpReadResponse),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] AccountAccessConsentAuthContext request)
    {
        IFluentResponse<AccountAccessConsentAuthContextCreateLocalResponse> fluentResponse = await _requestBuilder
            .AccountAndTransaction
            .AccountAccessConsents
            .AuthContexts
            .CreateLocalAsync(request);

        // HTTP response
        HttpResponse<AccountAccessConsentAuthContextCreateLocalResponse> httpResponseTmp =
            fluentResponse.ToHttpResponse();
        var httpResponse =
            new AccountAccessConsentAuthContextsHttpReadResponse(httpResponseTmp.Messages, httpResponseTmp.Data);
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<AccountAccessConsentAuthContextCreateLocalResponse> _ => StatusCodes.Status201Created,
            FluentBadRequestErrorResponse<AccountAccessConsentAuthContextCreateLocalResponse> _ => StatusCodes
                .Status400BadRequest,
            FluentOtherErrorResponse<AccountAccessConsentAuthContextCreateLocalResponse> _ => StatusCodes
                .Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
