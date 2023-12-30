// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.Management;

[ApiController]
[ApiExplorerSettings(GroupName = "manage")]
[Route("manage/obseal-certificates")]
[Tags("OBSeal Signing Certificates")]
public class ObSealCertificatesController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public ObSealCertificatesController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Add OBSeal signing certificate
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        ObSealCertificateResponse>> PostAsync([FromBody] ObSealCertificate request)
    {
        // Operation
        ObSealCertificateResponse fluentResponse = await _requestBuilder
            .Management
            .ObSealCertificates
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { obSealCertificateId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read OBSeal signing certificate
    /// </summary>
    /// <param name="obSealCertificateId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpGet("{obSealCertificateId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        ObSealCertificateResponse>> GetAsync(
        Guid obSealCertificateId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        ObSealCertificateResponse fluentResponse = await _requestBuilder
            .Management
            .ObSealCertificates
            .ReadLocalAsync(
                obSealCertificateId,
                modifiedBy);

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete OBSeal signing certificate
    /// </summary>
    /// <param name="obSealCertificateId"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    [HttpDelete("{obSealCertificateId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid obSealCertificateId,
        [FromHeader(Name = "x-obc-modified-by")]
        string? modifiedBy)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .Management
            .ObSealCertificates
            .DeleteLocalAsync(
                obSealCertificateId,
                modifiedBy);

        return Ok(fluentResponse);
    }
}
