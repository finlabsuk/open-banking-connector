// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.BankConfiguration;

[ApiController]
[ApiExplorerSettings(GroupName = "manage")]
[Route("manage/obwac-certificates")]
[Tags("OBWAC Transport Certificates")]
public class ObWacCertificatesController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public ObWacCertificatesController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Add OBWAC transport certificate
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        ObWacCertificateResponse>> PostAsync([FromBody] ObWacCertificate request)
    {
        // Operation
        ObWacCertificateResponse fluentResponse = await _requestBuilder
            .Management
            .ObWacCertificates
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { obWacCertificateId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read OBWAC transport certificate
    /// </summary>
    /// <param name="obWacCertificateId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{obWacCertificateId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        ObWacCertificateResponse>> GetAsync(
        Guid obWacCertificateId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        ObWacCertificateResponse fluentResponse = await _requestBuilder
            .Management
            .ObWacCertificates
            .ReadLocalAsync(
                obWacCertificateId,
                modifiedBy);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete OBWAC transport certificate
    /// </summary>
    /// <param name="obWacCertificateId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{obWacCertificateId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid obWacCertificateId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .Management
            .ObWacCertificates
            .DeleteLocalAsync(
                obWacCertificateId,
                modifiedBy);

        return Ok(fluentResponse);
    }
}
