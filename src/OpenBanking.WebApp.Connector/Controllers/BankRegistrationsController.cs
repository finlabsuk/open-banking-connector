// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Web.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers;

[ApiController]
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
    [Route("bank-registrations")]
    [HttpPost]
    [ProducesResponseType(
        typeof(HttpResponse<BankRegistrationResponse>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(HttpResponse<BankRegistrationResponse>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<BankRegistrationResponse>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAsync([FromBody] BankRegistration request)
    {
        // Operation
        IFluentResponse<BankRegistrationResponse> fluentResponse = await _requestBuilder
            .BankConfiguration
            .BankRegistrations
            .CreateAsync(request);

        // HTTP response
        HttpResponse<BankRegistrationResponse> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<BankRegistrationResponse> _ => StatusCodes.Status201Created,
            FluentBadRequestErrorResponse<BankRegistrationResponse> _ => StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<BankRegistrationResponse> _ => StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }

    /// <summary>
    ///     Read all BankRegistration objects
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [Route("bank-registrations")]
    [HttpGet]
    [ProducesResponseType(
        typeof(HttpResponse<IList<BankRegistrationResponse>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<BankRegistrationResponse>>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(HttpResponse<IList<BankRegistrationResponse>>),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAsync()
    {
        // Operation
        IFluentResponse<IQueryable<BankRegistrationResponse>> fluentResponse = await _requestBuilder
            .BankConfiguration
            .BankRegistrations
            .GetLocalAsync(query => true);

        // HTTP response
        HttpResponse<IQueryable<BankRegistrationResponse>> httpResponse = fluentResponse.ToHttpResponse();
        int statusCode = fluentResponse switch
        {
            FluentSuccessResponse<IQueryable<BankRegistrationResponse>> _ => StatusCodes.Status200OK,
            FluentBadRequestErrorResponse<IQueryable<BankRegistrationResponse>> _ =>
                StatusCodes.Status400BadRequest,
            FluentOtherErrorResponse<IQueryable<BankRegistrationResponse>> _ =>
                StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ObjectResult(httpResponse)
            { StatusCode = statusCode };
    }
}