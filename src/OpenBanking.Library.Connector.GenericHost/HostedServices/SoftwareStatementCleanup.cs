// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices;

public class SoftwareStatementCleanup
{
    public Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        ISecretProvider secretProvider,
        HttpClientSettings httpClientSettings,
        IInstrumentationClient instrumentationClient)
    {
        List<SoftwareStatementEntity> softwareStatementList =
            postgreSqlDbContext
                .SoftwareStatement
                .ToList();

        DbSet<ObWacCertificateEntity> obWacList =
            postgreSqlDbContext
                .ObWacCertificate;

        DbSet<ObSealCertificateEntity> obSealList =
            postgreSqlDbContext
                .ObSealCertificate;

        var obWacProfiles = new Dictionary<Guid, ProcessedTransportCertificateProfile?>();
        foreach (ObWacCertificateEntity obWac in obWacList)
        {
            ProcessedTransportCertificateProfile? processedTransportCertificateProfile;
            if (!secretProvider.TryGetSecret(obWac.AssociatedKey.Name, out string? associatedKey))
            {
                processedTransportCertificateProfile = null;
                string message =
                    $"OBWAC transport certificate with ID {obWac.Id} " +
                    $"specifies AssociatedKey with name {obWac.AssociatedKey.Name} " +
                    "but no such value can be found. Any software statement(s) depending " +
                    "on this OBWAC transport certificate will not be able to be used.";
                instrumentationClient.Warning(message);
            }
            else
            {
                processedTransportCertificateProfile = new ProcessedTransportCertificateProfile(
                    new TransportCertificateProfile
                    {
                        Active = true,
                        DisableTlsCertificateVerification = false,
                        Certificate = obWac.Certificate,
                        AssociatedKey = associatedKey
                    },
                    obWac.Id.ToString(),
                    null,
                    httpClientSettings.PooledConnectionLifetimeSeconds,
                    instrumentationClient);
            }
            obWacProfiles[obWac.Id] = processedTransportCertificateProfile;
        }

        var obSealProfiles = new Dictionary<Guid, ProcessedSigningCertificateProfile?>();
        foreach (ObSealCertificateEntity obSeal in obSealList)
        {
            ProcessedSigningCertificateProfile? processedSigningCertificateProfile;
            if (!secretProvider.TryGetSecret(obSeal.AssociatedKey.Name, out string? associatedKey))
            {
                processedSigningCertificateProfile = null;
                string message =
                    $"OBSeal signing certificate with ID {obSeal.Id} " +
                    $"specifies AssociatedKey with name {obSeal.AssociatedKey.Name} " +
                    "but no such value can be found. Any software statement(s) depending " +
                    "on this OBSeal signing certificate will not be able to be used.";
                instrumentationClient.Warning(message);
            }
            else
            {
                processedSigningCertificateProfile = new ProcessedSigningCertificateProfile(
                    new SigningCertificateProfile
                    {
                        Active = true,
                        AssociatedKey = associatedKey,
                        AssociatedKeyId = obSeal.AssociatedKeyId,
                        Certificate = obSeal.Certificate
                    },
                    obSeal.Id.ToString(),
                    instrumentationClient);
            }
            obSealProfiles[obSeal.Id] = processedSigningCertificateProfile;
        }

        foreach (SoftwareStatementEntity softwareStatement in softwareStatementList)
        {
            ProcessedTransportCertificateProfile? obWacProfile =
                obWacProfiles[softwareStatement.DefaultObWacCertificateId];
            ProcessedSigningCertificateProfile? obSealProfile =
                obSealProfiles[softwareStatement.DefaultObSealCertificateId];
            if (obWacProfile is null ||
                obSealProfile is null)
            {
                string message =
                    $"Software statement with ID {softwareStatement.Id} " +
                    $"cannot be used due to lack of suitable OBWAC transport " +
                    $"or OBSeal signing certificate. See previous warnings for specific issues.";
                instrumentationClient.Warning(message);
                continue;
            }

            processedSoftwareStatementProfileStore.AddProfile(
                obWacProfile,
                obSealProfile,
                new SoftwareStatementProfile
                {
                    Active = true,
                    OrganisationId = softwareStatement.OrganisationId,
                    SoftwareId = softwareStatement.SoftwareId,
                    SandboxEnvironment = softwareStatement.SandboxEnvironment,
                    TransportCertificateProfileId = softwareStatement.DefaultObWacCertificateId.ToString(), // ignored
                    SigningCertificateProfileId = softwareStatement.DefaultObSealCertificateId.ToString(), // ignored
                    DefaultQueryRedirectUrl = softwareStatement.DefaultQueryRedirectUrl,
                    DefaultFragmentRedirectUrl = softwareStatement.DefaultFragmentRedirectUrl
                },
                softwareStatement.Id.ToString(), // sets ID of profile added to store
                instrumentationClient);
        }
        return Task.CompletedTask;
    }
}
