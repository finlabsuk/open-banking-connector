// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.BankConfiguration;

[ApiController]
[ApiExplorerSettings(GroupName = "config")]
public class VariableRecurringPaymentsApisController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public VariableRecurringPaymentsApisController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create an VariableRecurringPaymentsApi object
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("config/account-and-transaction-apis")]
    [HttpPost]
    [ProducesResponseType(
        typeof(VariableRecurringPaymentsApiResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] VariableRecurringPaymentsApiRequest request)
    {
        IFluentResponse<VariableRecurringPaymentsApiResponse> fluentResponse = await _requestBuilder
            .BankConfiguration
            .VariableRecurringPaymentsApis
            .CreateLocalAsync(request);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<VariableRecurringPaymentsApiResponse> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<VariableRecurringPaymentsApiResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<VariableRecurringPaymentsApiResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    ///     Read all VariableRecurringPaymentsApi objects (temporary endpoint)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("config/account-and-transaction-apis")]
    [HttpGet]
    [ProducesResponseType(
        typeof(HttpResponse<IList<VariableRecurringPaymentsApiResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync()
    {
        // Operation
        IFluentResponse<IQueryable<VariableRecurringPaymentsApiResponse>> fluentResponse = await _requestBuilder
            .BankConfiguration
            .VariableRecurringPaymentsApis
            .ReadLocalAsync(query => true);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<IQueryable<VariableRecurringPaymentsApiResponse>> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<IQueryable<VariableRecurringPaymentsApiResponse>> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<IQueryable<VariableRecurringPaymentsApiResponse>> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
