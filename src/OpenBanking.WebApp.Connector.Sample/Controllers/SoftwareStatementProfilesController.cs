// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Sample.Controllers
{
    [ApiController]
    // ReSharper disable once InconsistentNaming
    public class SoftwareStatementController : ControllerBase
    {
        private readonly RuntimeConfiguration _config;
        private readonly BaseDbContext _dbContext;
        private readonly IKeySecretProvider _keySecrets;
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;
        private readonly ISoftwareStatementProfileRepository _profileRepo;

        public SoftwareStatementController(BaseDbContext dbContext, IConfigurationProvider configProvider,
            ISoftwareStatementProfileRepository profileRepo,
            IOpenBankingRequestBuilder obRequestBuilder, IKeySecretProvider keySecrets)
        {
            _dbContext = dbContext;
            _config = configProvider.GetRuntimeConfiguration();
            _profileRepo = profileRepo;
            _obRequestBuilder = obRequestBuilder;
            _keySecrets = keySecrets;
        }

        [Route("software-statement-profiles")]
        [HttpPost]
        [ProducesResponseType(typeof(PostSoftwareStatementProfileResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessagesResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSoftwareStatementProfileAsync(
            [FromBody] PostSoftwareStatementProfileRequest request)
        {
            var softwareStatement =
                await _keySecrets.GetKeySecretAsync(KeySecrets.GetName(request.SoftwareStatementProfileId,
                    KeySecrets.SoftwareStatement));
            var signingKeyId =
                await _keySecrets.GetKeySecretAsync(KeySecrets.GetName(request.SoftwareStatementProfileId,
                    KeySecrets.SigningKeyId));
            var signingCertificateKey =
                await _keySecrets.GetKeySecretAsync(KeySecrets.GetName(request.SoftwareStatementProfileId,
                    KeySecrets.SigningCertificateKey));
            var signingCertificate =
                await _keySecrets.GetKeySecretAsync(KeySecrets.GetName(request.SoftwareStatementProfileId,
                    KeySecrets.SigningCertificate));
            var transportCertificateKey = await _keySecrets.GetKeySecretAsync(
                KeySecrets.GetName(request.SoftwareStatementProfileId, KeySecrets.TransportCertificateKey));
            var transportCertificate =
                await _keySecrets.GetKeySecretAsync(KeySecrets.GetName(request.SoftwareStatementProfileId,
                    KeySecrets.TransportCertificate));

            var statementResp = await _obRequestBuilder.SoftwareStatementProfile()
                .SoftwareStatementProfileId(request.SoftwareStatementProfileId)
                .SoftwareStatement(softwareStatement?.Value)
                .SigningKeyId(signingKeyId?.Value)
                .SigningCertificate(signingCertificateKey?.Value, signingCertificate?.Value)
                .TransportCertificate(transportCertificateKey?.Value, transportCertificate?.Value)
                .DefaultFragmentRedirectUrl(_config.DefaultFragmentRedirectUrl)
                .SubmitAsync();

            var result = new PostSoftwareStatementProfileResponse(
                statementResp.ToMessagesResponse()
            );

            return statementResp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }


        [Route("software-statement-profiles")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<string>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MessagesResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetSoftwareStatementProfileIdsAsync()
        {
            var result = (await _profileRepo.GetIdsAsync()).OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase);

            return new OkObjectResult(result);
        }
    }
}
