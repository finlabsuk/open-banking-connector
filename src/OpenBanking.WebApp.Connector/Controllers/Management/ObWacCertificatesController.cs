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
    /// <returns></returns>
    [HttpGet("{obWacCertificateId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        ObWacCertificateResponse>> GetAsync(
        Guid obWacCertificateId)
    {
        // Operation
        ObWacCertificateResponse fluentResponse = await _requestBuilder
            .Management
            .ObWacCertificates
            .ReadLocalAsync(
                new LocalReadParams
                {
                    Id = obWacCertificateId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete OBWAC transport certificate
    /// </summary>
    /// <param name="obWacCertificateId"></param>
    /// <returns></returns>
    [HttpDelete("{obWacCertificateId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid obWacCertificateId)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .Management
            .ObWacCertificates
            .DeleteLocalAsync(
                new LocalDeleteParams
                {
                    Id = obWacCertificateId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }
}
