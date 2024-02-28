// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

namespace FinnovationLabs.OpenBanking.Library.Connector.Repositories;

public interface IProcessedSoftwareStatementProfileStore
{
    Task<ProcessedSoftwareStatementProfile> GetAsync(string? id, string? overrideVariant = null);

    ProcessedSoftwareStatementProfile GetProfile(
        ObWacCertificate processedTransportCertificateProfile,
        ObSealCertificate processedSigningCertificateProfile,
        SoftwareStatementProfile softwareStatementProfile,
        string id,
        IInstrumentationClient instrumentationClient);

    void AddProfile(
        ProcessedSoftwareStatementProfile defaultVariant,
        string id);
}

public class ProcessedSoftwareStatementProfileStore : IProcessedSoftwareStatementProfileStore
{
    private readonly ConcurrentDictionary<string, ProcessedSoftwareStatementProfiles> _cache = new(
        StringComparer.InvariantCultureIgnoreCase);

    public ProcessedSoftwareStatementProfileStore(
        ISettingsProvider<SoftwareStatementProfilesSettings> softwareStatementProfilesSettingsProvider,
        ISettingsProvider<TransportCertificateProfilesSettings> transportCertificateProfilesSettingsProvider,
        ISettingsProvider<SigningCertificateProfilesSettings> signingCertificateProfilesSettingsProvider,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        IInstrumentationClient instrumentationClient)
    {
        HttpClientSettings httpClientSettings = httpClientSettingsProvider.GetSettings();

        // Create signing cert profile cache
        SigningCertificateProfilesSettings signingCertificateProfilesSettings =
            signingCertificateProfilesSettingsProvider.GetSettings();
        signingCertificateProfilesSettings.ArgNotNull(nameof(signingCertificateProfilesSettings));
        ConcurrentDictionary<string, ObSealCertificate> scpCache =
            new(StringComparer.InvariantCultureIgnoreCase);
        foreach ((string scpId, SigningCertificateProfile inputSigningProfile) in
                 signingCertificateProfilesSettings)
        {
            // Ignore inactive profiles
            if (!inputSigningProfile.Active)
            {
                string message =
                    "Configuration/secrets warning: " +
                    $"Signing certificate profile with ID {scpId} " +
                    "in configuration/secrets is " +
                    $"specified with {nameof(inputSigningProfile.Active)} set to false and therefore ignored.";
                instrumentationClient.Warning(message);
                continue;
            }

            // Get default variant (no overrides)
            SigningCertificateProfile defaultTransportCertificateProfile =
                inputSigningProfile;
            var defaultVariant = new ObSealCertificate(
                defaultTransportCertificateProfile,
                scpId,
                instrumentationClient);

            // Add to cache
            scpCache[scpId] = defaultVariant;
        }

        // Create transport cert profile cache
        TransportCertificateProfilesSettings transportCertificateProfilesSettings =
            transportCertificateProfilesSettingsProvider.GetSettings();
        transportCertificateProfilesSettings.ArgNotNull(nameof(transportCertificateProfilesSettings));
        ConcurrentDictionary<string, ProcessedTransportCertificateProfiles> tcpCache =
            new(StringComparer.InvariantCultureIgnoreCase);
        foreach ((string tcpId, TransportCertificateProfileWithOverrideProperties unresolvedProfile) in
                 transportCertificateProfilesSettings)
        {
            // Ignore inactive profiles
            if (!unresolvedProfile.Active)
            {
                string message =
                    "Configuration/secrets warning: " +
                    $"Transport certificate profile with ID {tcpId} " +
                    "in configuration/secrets is " +
                    $"specified with {nameof(unresolvedProfile.Active)} set to false and therefore ignored.";
                instrumentationClient.Warning(message);
                continue;
            }

            // Get default variant
            TransportCertificateProfile defaultTransportCertificateProfile =
                unresolvedProfile.ApplyOverrides(null);
            var defaultVariant = new ObWacCertificate(
                defaultTransportCertificateProfile,
                tcpId,
                null,
                httpClientSettings.PooledConnectionLifetimeSeconds,
                instrumentationClient);

            // Get override cases (keys)
            var overrideCases =
                new HashSet<string>(unresolvedProfile.DisableTlsCertificateVerificationOverrides.Keys);

            // Get override variants
            ConcurrentDictionary<string, ObWacCertificate> overrideVariants =
                new(StringComparer.InvariantCultureIgnoreCase);
            foreach (string overrideCase in overrideCases)
            {
                TransportCertificateProfile overrideTransportCertificateProfile = unresolvedProfile
                    .ApplyOverrides(overrideCase);
                overrideVariants[overrideCase] = new ObWacCertificate(
                    overrideTransportCertificateProfile,
                    tcpId,
                    overrideCase,
                    httpClientSettings.PooledConnectionLifetimeSeconds,
                    instrumentationClient);
            }

            // Add to cache
            tcpCache[tcpId] = new ProcessedTransportCertificateProfiles(defaultVariant, overrideVariants);
        }

        // Create software statement profile cache
        SoftwareStatementProfilesSettings softwareStatementProfilesSettings =
            softwareStatementProfilesSettingsProvider.GetSettings();
        softwareStatementProfilesSettings.ArgNotNull(nameof(softwareStatementProfilesSettings));
        foreach ((string id, SoftwareStatementProfileWithOverrideProperties expandedProfile) in
                 softwareStatementProfilesSettings)
        {
            // Ignore inactive profiles
            if (!expandedProfile.Active)
            {
                string message =
                    "Configuration/secrets warning: " +
                    $"Software statement profile with ID {id} " +
                    "in configuration/secrets is " +
                    $"specified with {nameof(expandedProfile.Active)} set to false and therefore ignored.";
                instrumentationClient.Warning(message);
                continue;
            }

            // Get software statement profile (no override)
            SoftwareStatementProfile defaultSoftwareStatementProfile =
                expandedProfile.ApplyOverrides(null);

            // Get transport cert profile (no override)
            if (!tcpCache.TryGetValue(
                    defaultSoftwareStatementProfile.TransportCertificateProfileId,
                    out ProcessedTransportCertificateProfiles? defaultProcessedTransportCertificateProfiles))
            {
                string message1 =
                    "Configuration/secrets error: " +
                    $"Active software statement profile with ID {id} " +
                    $"sets {nameof(SoftwareStatementProfile.TransportCertificateProfileId)} " +
                    $"to {defaultSoftwareStatementProfile.TransportCertificateProfileId} " +
                    "but no such (active) transport certificate profile can be found.";
                throw new KeyNotFoundException(message1);
            }

            ObWacCertificate defaultProcessedTransportCertificateProfile =
                defaultProcessedTransportCertificateProfiles.DefaultVariant;

            // Get signing cert profile
            if (!scpCache.TryGetValue(
                    defaultSoftwareStatementProfile.SigningCertificateProfileId,
                    out ObSealCertificate? defaultProcessedSigningCertificateProfile))
            {
                string message1 =
                    "Configuration/secrets error: " +
                    $"Active software statement profile with ID {id} " +
                    $"sets {nameof(SoftwareStatementProfile.SigningCertificateProfileId)} " +
                    $"to {defaultSoftwareStatementProfile.SigningCertificateProfileId} " +
                    "but no such (active) signing certificate profile can be found.";
                throw new KeyNotFoundException(message1);
            }

            // Create default variant
            var defaultVariant =
                new ProcessedSoftwareStatementProfile(
                    defaultProcessedTransportCertificateProfile,
                    defaultProcessedSigningCertificateProfile,
                    defaultSoftwareStatementProfile,
                    id,
                    null,
                    instrumentationClient);

            // Get override cases (keys)
            var overrideCases =
                new HashSet<string>(expandedProfile.TransportCertificateProfileIdOverrides.Keys);
            overrideCases.UnionWith(expandedProfile.SigningCertificateProfileIdOverrides.Keys);
            overrideCases.UnionWith(defaultProcessedTransportCertificateProfiles.OverrideVariants.Keys);

            // Create and store override profiles
            ConcurrentDictionary<string, ProcessedSoftwareStatementProfile> overrideVariants =
                new(StringComparer.InvariantCultureIgnoreCase);
            foreach (string overrideCase in overrideCases)
            {
                // Get software statement profile (with override)
                SoftwareStatementProfile overrideSoftwareStatementProfile =
                    expandedProfile.ApplyOverrides(overrideCase);

                // Get transport cert profile (with override if available)
                if (!tcpCache.TryGetValue(
                        overrideSoftwareStatementProfile.TransportCertificateProfileId,
                        out ProcessedTransportCertificateProfiles? overrideResolvedTransportCertificateProfiles))
                {
                    string message =
                        "Configuration/secrets error: " +
                        $"Active software statement profile with ID {id} " +
                        $", for override {overrideCase}, " +
                        $"sets {nameof(SoftwareStatementProfile.TransportCertificateProfileId)} " +
                        $"to {overrideSoftwareStatementProfile.TransportCertificateProfileId} " +
                        "but no such (active) transport certificate profile can be found.";
                    throw new KeyNotFoundException(message);
                }

                if (!overrideResolvedTransportCertificateProfiles.OverrideVariants.TryGetValue(
                        overrideCase,
                        out ObWacCertificate? overrideTransportCertificateProfile))
                {
                    overrideTransportCertificateProfile =
                        overrideResolvedTransportCertificateProfiles.DefaultVariant;
                }

                // Get signing cert profile
                if (!scpCache.TryGetValue(
                        overrideSoftwareStatementProfile.SigningCertificateProfileId,
                        out ObSealCertificate? overrideProcessedSigningCertificateProfile))
                {
                    string message1 =
                        "Configuration/secrets error: " +
                        $"Active software statement profile with ID {id} " +
                        $"sets {nameof(SoftwareStatementProfile.SigningCertificateProfileId)} " +
                        $", for override {overrideCase}, " +
                        $"to {overrideSoftwareStatementProfile.SigningCertificateProfileId} " +
                        "but no such (active) signing certificate profile can be found.";
                    throw new KeyNotFoundException(message1);
                }

                // Create override variant
                overrideVariants[overrideCase] = new ProcessedSoftwareStatementProfile(
                    overrideTransportCertificateProfile,
                    overrideProcessedSigningCertificateProfile,
                    overrideSoftwareStatementProfile,
                    id,
                    overrideCase,
                    instrumentationClient);
            }

            // Add to cache
            _cache[id] = new ProcessedSoftwareStatementProfiles(defaultVariant, overrideVariants);
        }
    }

    public Task<ProcessedSoftwareStatementProfile> GetAsync(string? id, string? overrideVariant)
    {
        // Check for null
        if (id is null)
        {
            throw new KeyNotFoundException(
                "Bank registration does not specify software statement which normally means it was not available at application startup.");
        }

        // Load cache value
        if (!_cache.TryGetValue(id, out ProcessedSoftwareStatementProfiles? cacheValue))
        {
            throw new KeyNotFoundException($"Configuration/secrets error: Software Statement with ID {id} not found.");
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
                $"Configuration/secrets error: Software Statement Profile with ID {id} found but no override {overrideVariant} found for this software statement profile.");
        }

        return processedSoftwareStatementProfile.ToTaskResult();
    }

    public ProcessedSoftwareStatementProfile GetProfile(
        ObWacCertificate processedTransportCertificateProfile,
        ObSealCertificate processedSigningCertificateProfile,
        SoftwareStatementProfile softwareStatementProfile,
        string id,
        IInstrumentationClient instrumentationClient) =>
        // Create default variant
        new(
            processedTransportCertificateProfile,
            processedSigningCertificateProfile,
            softwareStatementProfile,
            id,
            null,
            instrumentationClient);


    public void AddProfile(
        ProcessedSoftwareStatementProfile defaultVariant,
        string id)
    {
        ConcurrentDictionary<string, ProcessedSoftwareStatementProfile> overrideVariants =
            new(StringComparer.InvariantCultureIgnoreCase);

        // Add to cache
        _cache[id] = new ProcessedSoftwareStatementProfiles(defaultVariant, overrideVariants);
    }
}
