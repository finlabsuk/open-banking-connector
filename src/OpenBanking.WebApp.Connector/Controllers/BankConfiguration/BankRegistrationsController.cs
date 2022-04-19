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
public class BankRegistrationsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public BankRegistrationsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Create a BankRegistration object
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("config/bank-registrations")]
    [HttpPost]
    [ProducesResponseType(
        typeof(BankRegistrationReadResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponseMessages),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] BankRegistration request)
    {
        // Operation
        IFluentResponse<BankRegistrationReadResponse> fluentResponse = await _requestBuilder
            .BankConfiguration
            .BankRegistrations
            .CreateAsync(request);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<BankRegistrationReadResponse> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<BankRegistrationReadResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<BankRegistrationReadResponse> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    ///     Read all BankRegistration objects (temporary endpoint)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("config/bank-registrations")]
    [HttpGet]
    [ProducesResponseType(
        typeof(IList<BankRegistrationReadLocalResponse>),
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
        IFluentResponse<IQueryable<BankRegistrationReadLocalResponse>> fluentResponse = await _requestBuilder
            .BankConfiguration
            .BankRegistrations
            .ReadLocalAsync(query => true);

        // HTTP response
        return fluentResponse switch
        {
            FluentSuccessResponse<IQueryable<BankRegistrationReadLocalResponse>> _ =>
                new ObjectResult(fluentResponse.Data!)
                    { StatusCode = StatusCodes.Status200OK },
            FluentBadRequestErrorResponse<IQueryable<BankRegistrationReadLocalResponse>> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status400BadRequest },
            FluentOtherErrorResponse<IQueryable<BankRegistrationReadLocalResponse>> _ =>
                new ObjectResult(fluentResponse.GetHttpResponseMessages() ?? new HttpResponseMessages())
                    { StatusCode = StatusCodes.Status500InternalServerError },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
