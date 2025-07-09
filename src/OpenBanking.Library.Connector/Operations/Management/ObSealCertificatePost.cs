// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Extensions.Caching.Memory;
using ObSealCertificateCached = FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management.ObSealCertificate;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class ObSealCertificatePost : IObjectCreate<ObSealCertificate,
    ObSealCertificateResponse, LocalCreateParams>
{
    private readonly IDbMethods _dbSaveChangesMethod;
    private readonly IDbEntityMethods<ObSealCertificateEntity> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ISecretProvider _secretProvider;
    private readonly ITimeProvider _timeProvider;

    public ObSealCertificatePost(
        IDbEntityMethods<ObSealCertificateEntity> entityMethods,
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IMemoryCache memoryCache,
        ISecretProvider secretProvider)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _instrumentationClient = instrumentationClient;
        _memoryCache = memoryCache;
        _secretProvider = secretProvider;
        _timeProvider = timeProvider;
        _entityMethods = entityMethods;
    }

    public async Task<(ObSealCertificateResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(
            ObSealCertificate request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new ObSealCertificateEntity(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            request.AssociatedKeyId,
            request.AssociatedKey,
            request.Certificate);

        // Add cache entry
        SecretResult associatedKeyResult = await _secretProvider.GetSecretAsync(entity.AssociatedKey);
        if (!associatedKeyResult.SecretObtained)
        {
            string fullMessage =
                $"Request specifies AssociatedKey with Source {entity.AssociatedKey.Source} " +
                $"and Name {entity.AssociatedKey.Name} which could not be obtained. {associatedKeyResult.ErrorMessage} " +
                "ObSealCertificate record will therefore not be created.";
            throw new KeyNotFoundException(fullMessage);
        }
        _memoryCache.Set(
            ObSealCertificateCached.GetCacheKey(entity.Id),
            ObSealCertificateCached.CreateInstance(
                entity,
                associatedKeyResult.Secret!,
                _instrumentationClient));

        // Add entity
        await _entityMethods.AddAsync(entity);

        // Create response
        ObSealCertificateResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
