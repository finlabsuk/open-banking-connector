// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;
using ObSealCertificateCache = FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache.ObSealCertificate;
using ObWacCertificateCache = FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache.ObWacCertificate;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class SoftwareStatementPost : IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams>
{
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IDbReadWriteEntityMethods<SoftwareStatementEntity> _entityMethods;
    private readonly ISettingsProvider<HttpClientSettings> _httpClientSettingsProvider;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
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
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        IMemoryCache memoryCache)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _instrumentationClient = instrumentationClient;
        _obSealMethods = obSealMethods;
        _obWacMethods = obWacMethods;
        _secretProvider = secretProvider;
        _httpClientSettingsProvider = httpClientSettingsProvider;
        _memoryCache = memoryCache;
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
        ProcessedTransportCertificateProfile processedTransportCertificateProfile =
            await ObWacCertificateCache.GetValue(
                entity.DefaultObWacCertificateId,
                _httpClientSettingsProvider.GetSettings(),
                _instrumentationClient,
                _secretProvider,
                _obWacMethods,
                _memoryCache);

        // Load related OBSeal
        Guid obSealId = entity.DefaultObSealCertificateId;
        ProcessedSigningCertificateProfile processedSigningCertificateProfile = await ObSealCertificateCache.GetValue(
            obSealId,
            _instrumentationClient,
            _secretProvider,
            _obSealMethods,
            _memoryCache);

        // Add software statement to cache
        var softwareStatementIdString = entity.Id.ToString();
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile = _softwareStatementProfileRepo.GetProfile(
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
            softwareStatementIdString, // sets ID of profile added to store
            _instrumentationClient);

        _softwareStatementProfileRepo.AddProfile(processedSoftwareStatementProfile, softwareStatementIdString);

        // Add entity
        await _entityMethods.AddAsync(entity);

        // Create response
        SoftwareStatementResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
