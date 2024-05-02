// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.Management;

[ApiController]
[ApiExplorerSettings(GroupName = "manage")]
[Route("manage/software-statements")]
[Tags("Software Statements")]
public class SoftwareStatementsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public SoftwareStatementsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Add software statement
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        SoftwareStatementResponse>> PostAsync([FromBody] SoftwareStatement request)
    {
        // Operation
        SoftwareStatementResponse fluentResponse = await _requestBuilder
            .Management
            .SoftwareStatements
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { softwareStatementId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read software statement
    /// </summary>
    /// <param name="softwareStatementId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{softwareStatementId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        SoftwareStatementResponse>> GetAsync(
        Guid softwareStatementId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        SoftwareStatementResponse fluentResponse = await _requestBuilder
            .Management
            .SoftwareStatements
            .ReadLocalAsync(
                softwareStatementId,
                modifiedBy);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Update software statement
    /// </summary>
    /// <param name="request"></param>
    /// <param name="softwareStatementId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpPut("{softwareStatementId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        SoftwareStatementResponse>> PutAsync(
        [FromBody]
        SoftwareStatementUpdate request,
        Guid softwareStatementId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        SoftwareStatementResponse fluentResponse = await _requestBuilder
            .Management
            .SoftwareStatements
            .UpdateAsync(
                request,
                new LocalReadParams
                {
                    Id = softwareStatementId,
                    ModifiedBy = modifiedBy
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete software statement
    /// </summary>
    /// <param name="softwareStatementId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{softwareStatementId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid softwareStatementId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .Management
            .SoftwareStatements
            .DeleteLocalAsync(
                softwareStatementId,
                modifiedBy);

        return Ok(fluentResponse);
    }
}
