﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;

internal class ObSealCertificatePost : IObjectCreate<ObSealCertificate, ObSealCertificateResponse, LocalCreateParams>
{
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IDbReadWriteEntityMethods<ObSealCertificateEntity> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private readonly ITimeProvider _timeProvider;

    public ObSealCertificatePost(
        IDbReadWriteEntityMethods<ObSealCertificateEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _instrumentationClient = instrumentationClient;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
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

        // Add entity
        await _entityMethods.AddAsync(entity);

        // Create response
        ObSealCertificateResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
