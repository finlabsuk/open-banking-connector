// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories
{
    public class ProcessedSoftwareStatementProfileStore : IReadOnlyRepository<ProcessedSoftwareStatementProfile>
    {
        private readonly MemoryRepository<ProcessedSoftwareStatementProfile> _memoryRepo =
            new MemoryRepository<ProcessedSoftwareStatementProfile>();

        public ProcessedSoftwareStatementProfileStore(
            ISettingsProvider<OpenBankingConnectorSettings> obcSettingsProvider,
            ISettingsProvider<SoftwareStatementProfilesSettings> softwareStatementProfilesSettingsProvider,
            ISettingsProvider<OBCertificateProfilesSettings> obCertificateProfileSettingsProvider,
            IInstrumentationClient instrumentationClient)
        {
            OpenBankingConnectorSettings obcSettings = obcSettingsProvider.GetSettings();
            obcSettings.ArgNotNull(nameof(obcSettings));

            SoftwareStatementProfilesSettings softwareStatementProfilesSettings =
                softwareStatementProfilesSettingsProvider.GetSettings();
            softwareStatementProfilesSettings.ArgNotNull(nameof(softwareStatementProfilesSettings));

            OBCertificateProfilesSettings obCertificateProfilesSettings =
                obCertificateProfileSettingsProvider.GetSettings();
            obCertificateProfilesSettings.ArgNotNull(nameof(softwareStatementProfilesSettings));

            List<string> activeProfileIds = obcSettings.ProcessedSoftwareStatementProfileIds.ToList();
            foreach (string softwareStatementProfileId in activeProfileIds)
            {
                // Get and validate software statement profile
                if (!softwareStatementProfilesSettings.TryGetValue(
                    softwareStatementProfileId,
                    out SoftwareStatementProfile softwareStatementProfile))
                {
                    throw new ArgumentOutOfRangeException(
                        $"Cannot find software statement profile with ID {softwareStatementProfileId}");
                }

                ValidationResult validationResult = new SoftwareStatementProfileValidator()
                    .Validate(softwareStatementProfile);
                validationResult.ProcessValidationResultsAndRaiseErrors(
                    "prefix",
                    "Validation failure when checking software statement profile.");

                // Get and validate OB certificate profile
                string obCertificateProfileId = softwareStatementProfile.OBCertificateProfileId;

                if (!obCertificateProfilesSettings.TryGetValue(
                    obCertificateProfileId,
                    out OBCertificateProfile obCertificateProfile))
                {
                    throw new ArgumentOutOfRangeException(
                        $"Cannot find OB certificate profile with ID {obCertificateProfileId}");
                }

                ValidationResult validationResult2 = new ObCertificateProfileValidator()
                    .Validate(obCertificateProfile);
                validationResult2.ProcessValidationResultsAndRaiseErrors(
                    "prefix",
                    "Validation failure when checking OB certificate profile.");

                // Create HttpMessageHandler with transport certificates
                var transportCerts = new List<X509Certificate2>();
                X509Certificate2 transportCert =
                    CertificateFactories.GetCertificate2FromPem(
                        obCertificateProfile.TransportKey,
                        obCertificateProfile.TransportCertificate) ??
                    throw new InvalidOperationException();
                transportCerts.Add(transportCert);

                IHttpRequestBuilder httpRequestBuilder = new HttpRequestBuilder()
                    .SetClientCertificates(transportCerts);
                if (obCertificateProfile.DisableTlsCertificateVerification)
                {
                    httpRequestBuilder
                        .SetServerCertificateValidator(new DefaultServerCertificateValidator());
                }

                HttpMessageHandler handler = httpRequestBuilder.CreateMessageHandler();

                // Add to cache
                var softwareStatementProfileCached =
                    new ProcessedSoftwareStatementProfile(
                        softwareStatementProfileId,
                        obCertificateProfile,
                        softwareStatementProfile,
                        new ApiClient(instrumentationClient, new HttpClient(handler)));

                _memoryRepo.SetAsync(softwareStatementProfileCached);
            }
        }

        public async Task<ProcessedSoftwareStatementProfile?> GetAsync(string id) => await _memoryRepo.GetAsync(id);

        public async Task<IQueryable<ProcessedSoftwareStatementProfile>> GetAsync(
            Expression<Func<ProcessedSoftwareStatementProfile, bool>> predicate) =>
            await _memoryRepo.GetAsync(predicate);

        public async Task<IList<string>> GetIdsAsync() =>
            await _memoryRepo.GetIdsAsync();
    }
}
