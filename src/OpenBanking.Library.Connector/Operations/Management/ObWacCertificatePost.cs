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
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class ObWacCertificatePost : IObjectCreate<ObWacCertificate, ObWacCertificateResponse, LocalCreateParams>
{
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IDbReadWriteEntityMethods<ObWacCertificateEntity> _entityMethods;
    private readonly ISettingsProvider<HttpClientSettings> _httpClientSettingsProvider;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ISecretProvider _secretProvider;
    private readonly ITimeProvider _timeProvider;

    public ObWacCertificatePost(
        IDbReadWriteEntityMethods<ObWacCertificateEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        ISettingsProvider<HttpClientSettings> httpClientSettingsProvider,
        IMemoryCache memoryCache,
        ISecretProvider secretProvider)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _instrumentationClient = instrumentationClient;
        _httpClientSettingsProvider = httpClientSettingsProvider;
        _memoryCache = memoryCache;
        _secretProvider = secretProvider;
        _timeProvider = timeProvider;
        _entityMethods = entityMethods;
    }

    public async Task<(ObWacCertificateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(
            ObWacCertificate request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new ObWacCertificateEntity(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            request.AssociatedKey,
            request.Certificate);

        // Add cache entry
        HttpClientSettings httpClientSettings = _httpClientSettingsProvider.GetSettings();
        ProcessedTransportCertificateProfile processedTransportCertificateProfile =
            ProcessedTransportCertificateProfile.GetProcessedObWac(
                _secretProvider,
                httpClientSettings,
                _instrumentationClient,
                entity);
        _memoryCache.Set(
            ProcessedTransportCertificateProfile.GetCacheKey(entity.Id),
            processedTransportCertificateProfile);

        // Add entity
        await _entityMethods.AddAsync(entity);

        // Create response
        ObWacCertificateResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
