// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public interface IProcessedSoftwareStatementProfileStore
    {
        Task<ProcessedSoftwareStatementProfile> GetAsync(string id, string? overrideVariant = null);
    }

    public class CacheValue
    {
        public CacheValue(ProcessedSoftwareStatementProfile defaultVariant)
        {
            DefaultVariant = defaultVariant;
        }

        public ProcessedSoftwareStatementProfile DefaultVariant { get; }

        public ConcurrentDictionary<string, ProcessedSoftwareStatementProfile> OverrideVariants { get; } =
            new ConcurrentDictionary<string, ProcessedSoftwareStatementProfile>(
                StringComparer.InvariantCultureIgnoreCase);
    }

    public class ProcessedSoftwareStatementProfileStore : IProcessedSoftwareStatementProfileStore
    {
        private readonly ConcurrentDictionary<string, CacheValue> _cache =
            new ConcurrentDictionary<string, CacheValue>(StringComparer.InvariantCultureIgnoreCase);

        public ProcessedSoftwareStatementProfileStore(
            ISettingsProvider<OpenBankingConnectorSettings> obcSettingsProvider,
            ISettingsProvider<SoftwareStatementProfilesSettings> softwareStatementProfilesSettingsProvider,
            ISettingsProvider<TransportCertificateProfilesSettings> transportCertificateProfilesSettingsProvider,
            ISettingsProvider<SigningCertificateProfilesSettings> signingCertificateProfilesSettingsProvider,
            IInstrumentationClient instrumentationClient)
        {
            OpenBankingConnectorSettings obcSettings = obcSettingsProvider.GetSettings();
            obcSettings.ArgNotNull(nameof(obcSettings));

            SoftwareStatementProfilesSettings softwareStatementProfilesSettings =
                softwareStatementProfilesSettingsProvider.GetSettings();
            softwareStatementProfilesSettings.ArgNotNull(nameof(softwareStatementProfilesSettings));

            TransportCertificateProfilesSettings transportCertificateProfilesSettings =
                transportCertificateProfilesSettingsProvider.GetSettings();
            transportCertificateProfilesSettings.ArgNotNull(nameof(transportCertificateProfilesSettings));

            SigningCertificateProfilesSettings signingCertificateProfilesSettings =
                signingCertificateProfilesSettingsProvider.GetSettings();
            signingCertificateProfilesSettings.ArgNotNull(nameof(signingCertificateProfilesSettings));

            List<string> softwareStatementProfileIds = obcSettings.SoftwareStatementProfileIdsAsList;
            List<string> softwareStatementAndCertificateOverrideCases =
                obcSettings.SoftwareStatementAndCertificateOverrideCasesAsList;

            foreach (string softwareStatementProfileId in softwareStatementProfileIds)
            {
                // Create and store default profile
                ProcessedSoftwareStatementProfile defaultProfile =
                    ProcessedSoftwareStatementProfile(
                        softwareStatementProfileId,
                        null);
                var cacheValue = new CacheValue(defaultProfile);
                if (!_cache.TryAdd(softwareStatementProfileId, cacheValue))
                {
                    throw new InvalidOperationException(
                        $"Software statement profile with ID {softwareStatementProfileId} already exists. Is this ID duplicated?");
                }

                // Create and store override profiles
                foreach (string softwareStatementAndCertificateOverrideCase in
                    softwareStatementAndCertificateOverrideCases)
                {
                    ProcessedSoftwareStatementProfile profile =
                        ProcessedSoftwareStatementProfile(
                            softwareStatementProfileId,
                            softwareStatementAndCertificateOverrideCase);

                    if (!cacheValue.OverrideVariants.TryAdd(
                        softwareStatementAndCertificateOverrideCase,
                        profile))
                    {
                        throw new InvalidOperationException(
                            $"Override case {softwareStatementAndCertificateOverrideCase} already exists. Is this case duplicated?");
                    }
                }
            }

            ProcessedSoftwareStatementProfile ProcessedSoftwareStatementProfile(
                string s,
                string? softwareStatementAndCertificateOverrideCase)
            {
                // Get software statement profile and apply overrides
                if (!softwareStatementProfilesSettings.TryGetValue(
                    s,
                    out SoftwareStatementProfileWithOverrideProperties
                        softwareStatementProfileWithOverrideProperties))
                {
                    throw new ArgumentOutOfRangeException(
                        $"No software statement profile with ID {s} supplied in key secrets.");
                }

                SoftwareStatementProfile softwareStatementProfile = softwareStatementProfileWithOverrideProperties
                    .ApplyOverrides(softwareStatementAndCertificateOverrideCase);

                // Validate software statement profile
                ValidationResult validationResult = new SoftwareStatementProfileValidator()
                    .Validate(softwareStatementProfile);
                validationResult.ProcessValidationResultsAndRaiseErrors(
                    "prefix",
                    "Validation failure when checking software statement profile.");

                // Get transport certificate profile and apply overrides
                string transportCertificateProfileId = softwareStatementProfile.TransportCertificateProfileId;
                if (!transportCertificateProfilesSettings.TryGetValue(
                    transportCertificateProfileId,
                    out TransportCertificateProfileWithOverrideProperties
                        transportCertificateProfileWithOverrideProperties))
                {
                    throw new ArgumentOutOfRangeException(
                        $"No transport certificate profile with ID {transportCertificateProfileId} supplied in key secrets.");
                }

                TransportCertificateProfile transportCertificateProfile =
                    transportCertificateProfileWithOverrideProperties
                        .ApplyOverrides(softwareStatementAndCertificateOverrideCase);

                // Validate transport certificate profile
                ValidationResult validationResult2 = new OBTransportCertificateProfileValidator()
                    .Validate(transportCertificateProfile);
                validationResult2.ProcessValidationResultsAndRaiseErrors(
                    "prefix",
                    "Validation failure when checking transport certificate profile.");

                // Get and validate OB signing certificate profile
                string signingCertificateProfileId = softwareStatementProfile.SigningCertificateProfileId;

                if (!signingCertificateProfilesSettings.TryGetValue(
                    signingCertificateProfileId,
                    out SigningCertificateProfile signingCertificateProfile))
                {
                    throw new ArgumentOutOfRangeException(
                        $"No signing certificate profile with ID {signingCertificateProfileId} supplied in key secrets.");
                }

                ValidationResult validationResult3 = new OBSigningCertificateProfileValidator()
                    .Validate(signingCertificateProfile);
                validationResult3.ProcessValidationResultsAndRaiseErrors(
                    "prefix",
                    "Validation failure when checking signing certificate profile.");

                // Create HttpMessageHandler with transport certificates
                var transportCerts = new List<X509Certificate2>();
                X509Certificate2 transportCert =
                    CertificateFactories.GetCertificate2FromPem(
                        transportCertificateProfile.AssociatedKey,
                        transportCertificateProfile.Certificate) ??
                    throw new InvalidOperationException();
                transportCerts.Add(transportCert);

                IHttpRequestBuilder httpRequestBuilder = new HttpRequestBuilder()
                    .SetClientCertificates(transportCerts);
                if (transportCertificateProfile.DisableTlsCertificateVerification)
                {
                    httpRequestBuilder
                        .SetServerCertificateValidator(new DefaultServerCertificateValidator());
                }

                HttpMessageHandler handler = httpRequestBuilder.CreateMessageHandler();

                var processedSoftwareStatementProfile = new ProcessedSoftwareStatementProfile(
                    s,
                    transportCertificateProfile,
                    signingCertificateProfile,
                    softwareStatementProfile,
                    new ApiClient(instrumentationClient, new HttpClient(handler)));
                return processedSoftwareStatementProfile;
            }
        }

        public Task<ProcessedSoftwareStatementProfile> GetAsync(string id, string? overrideVariant)
        {
            // Load cache value
            if (!_cache.TryGetValue(id, out CacheValue cacheValue))
            {
                throw new KeyNotFoundException($"Software statement profile with ID {id} not found.");
            }

            // Return default case where no override case
            if (overrideVariant is null)
            {
                return cacheValue.DefaultVariant.ToTaskResult();
            }

            // Return override case or throw exception if not available
            if (!cacheValue.OverrideVariants.TryGetValue(
                overrideVariant,
                out ProcessedSoftwareStatementProfile processedSoftwareStatementProfile))
            {
                throw new KeyNotFoundException(
                    $"Software statement profile with ID {id} found but no override case {overrideVariant} found for this software statement profile.");
            }

            return processedSoftwareStatementProfile.ToTaskResult();
        }
    }
}
