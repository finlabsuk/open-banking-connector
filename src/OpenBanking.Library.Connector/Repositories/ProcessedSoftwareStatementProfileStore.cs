// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
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
            new(StringComparer.InvariantCultureIgnoreCase);
    }

    public class ProcessedSoftwareStatementProfileStore : IProcessedSoftwareStatementProfileStore
    {
        private readonly ConcurrentDictionary<string, CacheValue> _cache = new(
            StringComparer.InvariantCultureIgnoreCase);

        public ProcessedSoftwareStatementProfileStore(
            ISettingsProvider<SoftwareStatementAndCertificateProfileOverridesSettings>
                softwareStatementAndCertificateProfileOverridesSettingsProvider,
            ISettingsProvider<SoftwareStatementProfilesSettings> softwareStatementProfilesSettingsProvider,
            ISettingsProvider<TransportCertificateProfilesSettings> transportCertificateProfilesSettingsProvider,
            ISettingsProvider<SigningCertificateProfilesSettings> signingCertificateProfilesSettingsProvider,
            IInstrumentationClient instrumentationClient)
        {
            SoftwareStatementAndCertificateProfileOverridesSettings softwareStatementAndCertificateProfileOverrides =
                softwareStatementAndCertificateProfileOverridesSettingsProvider.GetSettings();
            softwareStatementAndCertificateProfileOverrides.ArgNotNull(
                nameof(softwareStatementAndCertificateProfileOverrides));

            SoftwareStatementProfilesSettings softwareStatementProfilesSettings =
                softwareStatementProfilesSettingsProvider.GetSettings();
            softwareStatementProfilesSettings.ArgNotNull(nameof(softwareStatementProfilesSettings));

            TransportCertificateProfilesSettings transportCertificateProfilesSettings =
                transportCertificateProfilesSettingsProvider.GetSettings();
            transportCertificateProfilesSettings.ArgNotNull(nameof(transportCertificateProfilesSettings));

            SigningCertificateProfilesSettings signingCertificateProfilesSettings =
                signingCertificateProfilesSettingsProvider.GetSettings();
            signingCertificateProfilesSettings.ArgNotNull(nameof(signingCertificateProfilesSettings));

            foreach ((string id, SoftwareStatementProfileWithOverrideProperties rawProfile) in
                     softwareStatementProfilesSettings)
            {
                // Abort if profile inactive
                if (!rawProfile.Active)
                {
                    instrumentationClient.Info($"Ignoring inactive software statement profile with ID {id}.");
                    continue;
                }

                // Create and store default profile
                ProcessedSoftwareStatementProfile defaultProfile =
                    CreateProcessedSoftwareStatementProfile(
                        id,
                        rawProfile,
                        null,
                        transportCertificateProfilesSettings,
                        signingCertificateProfilesSettings,
                        instrumentationClient);
                var cacheValue = new CacheValue(defaultProfile);
                if (!_cache.TryAdd(id, cacheValue))
                {
                    throw new InvalidOperationException(
                        $"Software statement profile with ID {id} already exists. Is ID {id} duplicated?");
                }

                // Create and store override profiles
                foreach (string softwareStatementAndCertificateOverrideCase in
                         softwareStatementAndCertificateProfileOverrides)
                {
                    ProcessedSoftwareStatementProfile profile =
                        CreateProcessedSoftwareStatementProfile(
                            id,
                            rawProfile,
                            softwareStatementAndCertificateOverrideCase,
                            transportCertificateProfilesSettings,
                            signingCertificateProfilesSettings,
                            instrumentationClient);

                    if (!cacheValue.OverrideVariants.TryAdd(
                            softwareStatementAndCertificateOverrideCase,
                            profile))
                    {
                        throw new InvalidOperationException(
                            $"Software statement profile with ID {id} and override case {softwareStatementAndCertificateOverrideCase} already exists. " +
                            $"Is override case {softwareStatementAndCertificateOverrideCase} duplicated?");
                    }
                }
            }
        }

        public Task<ProcessedSoftwareStatementProfile> GetAsync(string id, string? overrideVariant)
        {
            // Load cache value
            if (!_cache.TryGetValue(id, out CacheValue? cacheValue))
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
                    out ProcessedSoftwareStatementProfile? processedSoftwareStatementProfile))
            {
                throw new KeyNotFoundException(
                    $"Software statement profile with ID {id} found but no override case {overrideVariant} found for this software statement profile.");
            }

            return processedSoftwareStatementProfile.ToTaskResult();
        }

        private ProcessedSoftwareStatementProfile CreateProcessedSoftwareStatementProfile(
            string id,
            SoftwareStatementProfileWithOverrideProperties softwareStatementProfileWithOverrideProperties,
            string? softwareStatementAndCertificateOverrideCase,
            TransportCertificateProfilesSettings transportCertificateProfilesSettings,
            SigningCertificateProfilesSettings signingCertificateProfilesSettings,
            IInstrumentationClient instrumentationClient)
        {
            // Log output
            instrumentationClient.Info(
                $"Processing active software statement profile with ID {id}" +
                (softwareStatementAndCertificateOverrideCase is null
                    ? "."
                    : $" and override case {softwareStatementAndCertificateOverrideCase}."
                ));

            // Apply overrides to software statement profile
            SoftwareStatementProfile softwareStatementProfile =
                softwareStatementProfileWithOverrideProperties
                    .ApplyOverrides(softwareStatementAndCertificateOverrideCase);

            // Validate software statement profile
            ValidationResult validationResult = new SoftwareStatementProfileValidator()
                .Validate(softwareStatementProfile);
            validationResult.ProcessValidationResultsAndRaiseErrors(
                "prefix",
                $"Validation failure when checking software statement profile with ID {id}" +
                (softwareStatementAndCertificateOverrideCase is null
                    ? "."
                    : $" and override case {softwareStatementAndCertificateOverrideCase}."
                ));

            // Get transport certificate profile and apply overrides
            string transportCertificateProfileId = softwareStatementProfile.TransportCertificateProfileId;
            if (!transportCertificateProfilesSettings.TryGetValue(
                    transportCertificateProfileId,
                    out TransportCertificateProfileWithOverrideProperties?
                        transportCertificateProfileWithOverrideProperties))
            {
                throw new ArgumentOutOfRangeException(
                    $"No transport certificate profile with ID {transportCertificateProfileId} found in configuration/key secrets.");
            }

            if (!transportCertificateProfileWithOverrideProperties.Active)
            {
                throw new ArgumentOutOfRangeException(
                    $"Transport certificate profile with ID {transportCertificateProfileId} is inactive yet specified by " +
                    $"software statement profile with ID {id}" +
                    (softwareStatementAndCertificateOverrideCase is null
                        ? "."
                        : $" and override case {softwareStatementAndCertificateOverrideCase}."
                    ));
            }

            TransportCertificateProfile transportCertificateProfile =
                transportCertificateProfileWithOverrideProperties
                    .ApplyOverrides(softwareStatementAndCertificateOverrideCase);

            // Validate transport certificate profile
            ValidationResult validationResult2 = new OBTransportCertificateProfileValidator()
                .Validate(transportCertificateProfile);
            validationResult2.ProcessValidationResultsAndRaiseErrors(
                "prefix",
                $"Validation failure when checking transport certificate profile with ID {transportCertificateProfileId}" +
                (softwareStatementAndCertificateOverrideCase is null
                    ? "."
                    : $" and override case {softwareStatementAndCertificateOverrideCase}."
                ));

            // Get and validate OB signing certificate profile
            string signingCertificateProfileId = softwareStatementProfile.SigningCertificateProfileId;
            if (!signingCertificateProfilesSettings.TryGetValue(
                    signingCertificateProfileId,
                    out SigningCertificateProfile? signingCertificateProfile))
            {
                throw new ArgumentOutOfRangeException(
                    $"No signing certificate profile with ID {signingCertificateProfileId} found in configuration/key secrets.");
            }

            if (!signingCertificateProfile.Active)
            {
                throw new ArgumentOutOfRangeException(
                    $"Signing certificate profile with ID {signingCertificateProfileId} is inactive yet specified by " +
                    $"software statement profile with ID {id}" +
                    (softwareStatementAndCertificateOverrideCase is null
                        ? "."
                        : $" and override case {softwareStatementAndCertificateOverrideCase}."
                    ));
            }

            // Validate signing certificate profile
            ValidationResult validationResult3 = new OBSigningCertificateProfileValidator()
                .Validate(signingCertificateProfile);
            validationResult3.ProcessValidationResultsAndRaiseErrors(
                "prefix",
                $"Validation failure when checking signing certificate profile with ID {signingCertificateProfileId}.");

            // Create HttpMessageHandler with transport certificates
            var transportCerts = new List<X509Certificate2>();
            X509Certificate2 transportCert =
                CertificateFactories.GetCertificate2FromPem(
                    transportCertificateProfile.AssociatedKey,
                    transportCertificateProfile.Certificate) ??
                throw new ArgumentException(
                    $"Encountered problem when processing transport certificate from transport certificate profile with ID {transportCertificateProfileId}" +
                    (softwareStatementAndCertificateOverrideCase is null
                        ? "."
                        : $" and override case {softwareStatementAndCertificateOverrideCase}."
                    ));
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
                id,
                transportCertificateProfile,
                signingCertificateProfile,
                softwareStatementProfile,
                new ApiClient(instrumentationClient, new HttpClient(handler)));
            return processedSoftwareStatementProfile;
        }
    }
}
