// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AccountAndTransaction;

[ApiController]
[ApiExplorerSettings(GroupName = "aisp")]
public class TransactionsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public TransactionsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Get Transactions
    /// </summary>
    /// <param name="accountAccessConsentId">ID for AccountAccessConsent returned by Open Banking Connector</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("aisp/transactions")]
    [HttpGet]
    [ProducesResponseType(
        typeof(HttpResponse<TransactionsResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponse<TransactionsResponse>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<TransactionsResponse>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync(
        [FromHeader(Name = "x-obc-account-access-consent-id")] [Required] string accountAccessConsentId)
    {
        // Operation
        IFluentResponse<TransactionsResponse> fluentResponse = null!;

        // HTTP response
        HttpResponse<TransactionsResponse> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<TransactionsResponse> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<TransactionsResponse> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<TransactionsResponse> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}
