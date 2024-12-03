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
[Route("manage/encryption-key-descriptions")]
[Tags("Encryption Key Descriptions")]
public class EncryptionKeyDescriptionsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public EncryptionKeyDescriptionsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Add encryption key description
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        EncryptionKeyDescriptionResponse>> PostAsync([FromBody] EncryptionKeyDescription request)
    {
        // Operation
        EncryptionKeyDescriptionResponse fluentResponse = await _requestBuilder
            .Management
            .EncryptionKeyDescriptions
            .CreateLocalAsync(request);

        return CreatedAtAction(
            nameof(GetAsync),
            new { encryptionKeyDescriptionId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read encryption key description
    /// </summary>
    /// <param name="encryptionKeyDescriptionId"></param>
    /// <returns></returns>
    [HttpGet("{encryptionKeyDescriptionId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        EncryptionKeyDescriptionResponse>> GetAsync(
        Guid encryptionKeyDescriptionId)
    {
        // Operation
        EncryptionKeyDescriptionResponse fluentResponse = await _requestBuilder
            .Management
            .EncryptionKeyDescriptions
            .ReadLocalAsync(
                new LocalReadParams
                {
                    Id = encryptionKeyDescriptionId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete encryption key description
    /// </summary>
    /// <param name="encryptionKeyDescriptionId"></param>
    /// <returns></returns>
    [HttpDelete("{encryptionKeyDescriptionId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid encryptionKeyDescriptionId)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .Management
            .EncryptionKeyDescriptions
            .DeleteLocalAsync(
                new LocalDeleteParams
                {
                    Id = encryptionKeyDescriptionId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }
}
