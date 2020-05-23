// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
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
        private readonly IKeySecretReadOnlyProvider _keySecrets;
        private readonly IOpenBankingRequestBuilder _obRequestBuilder;
        private readonly IDbEntityRepository<SoftwareStatementProfile> _profileRepo;

        public SoftwareStatementController(
            IConfigurationProvider configProvider,
            IDbEntityRepository<SoftwareStatementProfile> profileRepo,
            IOpenBankingRequestBuilder obRequestBuilder,
            IKeySecretReadOnlyProvider keySecrets)
        {
            _config = configProvider.GetRuntimeConfiguration();
            _profileRepo = profileRepo;
            _obRequestBuilder = obRequestBuilder;
            _keySecrets = keySecrets;
        }

        [Route("software-statement-profiles")]
        [HttpPost]
        [ProducesResponseType(
            type: typeof(PostSoftwareStatementProfileResponse),
            statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(MessagesResponse), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostSoftwareStatementProfileAsync(
            [FromBody] Library.Connector.Models.Public.Request.SoftwareStatementProfile request)
        {
            var softwareStatement =
                await _keySecrets.GetKeySecretAsync(
                    Secrets.GetName(
                        softwareStatementId: request.Id,
                        name: Secrets.SoftwareStatement));
            var signingKeyId =
                await _keySecrets.GetKeySecretAsync(
                    Secrets.GetName(
                        softwareStatementId: request.Id,
                        name: Secrets.SigningKeyId));
            var signingCertificateKey =
                await _keySecrets.GetKeySecretAsync(
                    Secrets.GetName(
                        softwareStatementId: request.Id,
                        name: Secrets.SigningCertificateKey));
            var signingCertificate =
                await _keySecrets.GetKeySecretAsync(
                    Secrets.GetName(
                        softwareStatementId: request.Id,
                        name: Secrets.SigningCertificate));
            var transportCertificateKey = await _keySecrets.GetKeySecretAsync(
                Secrets.GetName(softwareStatementId: request.Id, name: Secrets.TransportCertificateKey));
            var transportCertificate =
                await _keySecrets.GetKeySecretAsync(
                    Secrets.GetName(
                        softwareStatementId: request.Id,
                        name: Secrets.TransportCertificate));

            var statementResp = await _obRequestBuilder.SoftwareStatementProfile()
                .Id(request.Id)
                .SoftwareStatement(softwareStatement?.Value)
                .SigningKeyInfo(
                    keyId: signingKeyId?.Value,
                    keySecretName: signingCertificateKey?.Value,
                    certificate: signingCertificate?.Value)
                .TransportKeyInfo(
                    keySecretName: transportCertificateKey?.Value,
                    certificate: transportCertificate?.Value)
                .DefaultFragmentRedirectUrl(request.DefaultFragmentRedirectUrl)
                .SubmitAsync();

            var result = new PostSoftwareStatementProfileResponse(statementResp.ToMessagesResponse());

            return statementResp.HasErrors
                ? new BadRequestObjectResult(result.Messages) as IActionResult
                : new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }


        [Route("software-statement-profiles")]
        [HttpGet]
        [ProducesResponseType(type: typeof(IList<string>), statusCode: (int) HttpStatusCode.OK)]
        [ProducesResponseType(type: typeof(MessagesResponse), statusCode: (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetSoftwareStatementProfileIdsAsync()
        {
            var result = (await _profileRepo.GetAllAsync()).Select(x => x.Id).OrderBy(
                keySelector: x => x,
                comparer: StringComparer.InvariantCultureIgnoreCase);

            return new OkObjectResult(result);
        }
    }
}
