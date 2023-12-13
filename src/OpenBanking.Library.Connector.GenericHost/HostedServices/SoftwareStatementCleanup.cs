// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices;

public class SoftwareStatementCleanup
{
    public Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        IConfiguration configuration,
        HttpClientSettings httpClientSettings,
        IInstrumentationClient instrumentationClient)
    {
        List<SoftwareStatementEntity> entityList =
            postgreSqlDbContext
                .SoftwareStatement
                .ToList();

        DbSet<ObWacCertificateEntity> obwacList =
            postgreSqlDbContext
                .ObWacCertificate;

        DbSet<ObSealCertificateEntity> obsealList =
            postgreSqlDbContext
                .ObSealCertificate;

        var obWacProfiles = new Dictionary<Guid, ProcessedTransportCertificateProfile>();
        foreach (ObWacCertificateEntity x in obwacList)
        {
            var y = new TransportCertificateProfile
            {
                Active = true,
                DisableTlsCertificateVerification = false,
                AssociatedKey = GetConfigurationValue(x.AssociatedKey.Name, configuration),
                Certificate = x.Certificate
            };
            var z = new ProcessedTransportCertificateProfile(
                y,
                x.Id.ToString(),
                null,
                httpClientSettings.PooledConnectionLifetimeSeconds,
                instrumentationClient);
            obWacProfiles[x.Id] = z;
        }

        var obSealProfiles = new Dictionary<Guid, ProcessedSigningCertificateProfile>();
        foreach (ObSealCertificateEntity x in obsealList)
        {
            var y = new SigningCertificateProfile
            {
                Active = true,
                AssociatedKey = GetConfigurationValue(x.AssociatedKey.Name, configuration),
                AssociatedKeyId = x.AssociatedKeyId,
                Certificate = x.Certificate
            };
            var z = new ProcessedSigningCertificateProfile(y, x.Id.ToString(), instrumentationClient);
            obSealProfiles[x.Id] = z;
        }

        foreach (SoftwareStatementEntity x in entityList)
        {
            ProcessedTransportCertificateProfile obWacProfile = obWacProfiles[x.DefaultObWacCertificateId];
            ProcessedSigningCertificateProfile obSealProfile = obSealProfiles[x.DefaultObSealCertificateId];
            var old = new SoftwareStatementProfile
            {
                Active = true,
                OrganisationId = x.OrganisationId,
                SoftwareId = x.SoftwareId,
                SandboxEnvironment = x.SandboxEnvironment,
                TransportCertificateProfileId = x.DefaultObWacCertificateId.ToString(), // ignored
                SigningCertificateProfileId = x.DefaultObSealCertificateId.ToString(), // ignored
                DefaultQueryRedirectUrl = x.DefaultQueryRedirectUrl,
                DefaultFragmentRedirectUrl = x.DefaultFragmentRedirectUrl
            };

            processedSoftwareStatementProfileStore.AddProfile(
                obWacProfile,
                obSealProfile,
                old,
                x.Id.ToString(), // sets ID of profile added to store
                instrumentationClient);
        }
        return Task.CompletedTask;
    }

    private static string GetConfigurationValue(string name, IConfiguration configuration)
    {
        var value = configuration.GetValue<string>(name, "");
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Cannot get non-empty value from specified configuration setting {name}.");
        }


        return value;
    }
}
