// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;

internal class SoftwareStatementPost : IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams>
{
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IDbReadWriteEntityMethods<SoftwareStatementEntity> _entityMethods;
    private readonly ISettingsProvider<HttpClientSettings> _httpClientSettingsProvider;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbReadWriteEntityMethods<ObSealCertificateEntity> _obSealMethods;
    private readonly IDbReadWriteEntityMethods<ObWacCertificateEntity> _obWacMethods;
    private readonly ISecretProvider _secretProvider;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private readonly ITimeProvider _timeProvider;

    public SoftwareStatementPost(
        IDbReadWriteEntityMethods<SoftwareStatementEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IDbReadWriteEntityMethods<ObSealCertificateEntity> obSealMethods,
        IDbReadWriteEntityMethods<ObWacCertificateEntity> obWacMethods,
        ISecretProvider secretProvider,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _instrumentationClient = instrumentationClient;
        _obSealMethods = obSealMethods;
        _obWacMethods = obWacMethods;
        _secretProvider = secretProvider;
        _httpClientSettingsProvider = httpClientSettingsProvider;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _timeProvider = timeProvider;
        _entityMethods = entityMethods;
    }

    public async Task<(SoftwareStatementResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(
            SoftwareStatement request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new SoftwareStatementEntity(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            request.OrganisationId,
            request.SoftwareId,
            request.SandboxEnvironment,
            request.DefaultObWacCertificateId,
            request.DefaultObSealCertificateId,
            request.DefaultQueryRedirectUrl,
            request.DefaultFragmentRedirectUrl);

        // Load related OBWAC
        HttpClientSettings httpClientSettings = _httpClientSettingsProvider.GetSettings();
        ObWacCertificateEntity obWac =
            await _obWacMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == entity.DefaultObWacCertificateId) ??
            throw new KeyNotFoundException(
                $"No record found for ObWacCertificate with ID {entity.DefaultObWacCertificateId}.");
        var processedTransportCertificateProfile = new ProcessedTransportCertificateProfile(
            new TransportCertificateProfile
            {
                Active = true,
                DisableTlsCertificateVerification = false,
                AssociatedKey = _secretProvider.GetSecret(obWac.AssociatedKey.Name),
                Certificate = obWac.Certificate
            },
            obWac.Id.ToString(),
            null,
            httpClientSettings.PooledConnectionLifetimeSeconds,
            _instrumentationClient);

        // Load related OBSeal
        ObSealCertificateEntity obSeal =
            await _obSealMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == entity.DefaultObSealCertificateId) ??
            throw new KeyNotFoundException(
                $"No record found for ObSealCertificate with ID {entity.DefaultObSealCertificateId}.");
        var processedSigningCertificateProfile = new ProcessedSigningCertificateProfile(
            new SigningCertificateProfile
            {
                Active = true,
                AssociatedKey = _secretProvider.GetSecret(obSeal.AssociatedKey.Name),
                AssociatedKeyId = obSeal.AssociatedKeyId,
                Certificate = obSeal.Certificate
            },
            obSeal.Id.ToString(),
            _instrumentationClient);

        // Add software statement to cache
        _softwareStatementProfileRepo.AddProfile(
            processedTransportCertificateProfile,
            processedSigningCertificateProfile,
            new SoftwareStatementProfile
            {
                Active = true,
                OrganisationId = entity.OrganisationId,
                SoftwareId = entity.SoftwareId,
                SandboxEnvironment = entity.SandboxEnvironment,
                TransportCertificateProfileId = entity.DefaultObWacCertificateId.ToString(), // ignored
                SigningCertificateProfileId = entity.DefaultObSealCertificateId.ToString(), // ignored
                DefaultQueryRedirectUrl = entity.DefaultQueryRedirectUrl,
                DefaultFragmentRedirectUrl = entity.DefaultFragmentRedirectUrl
            },
            entity.Id.ToString(), // sets ID of profile added to store
            _instrumentationClient);
        
        // Add entity
        await _entityMethods.AddAsync(entity);

        // Create response
        SoftwareStatementResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
