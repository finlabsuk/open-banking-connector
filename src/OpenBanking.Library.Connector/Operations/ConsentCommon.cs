﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class
    ConsentCommon<TEntity, TPublicRequest, TPublicResponse, TApiRequest, TApiResponse>
    where TEntity : class, IEntity
    where TPublicRequest : ConsentBase
    where TApiRequest : class
    where TApiResponse : class, ISupportsValidation
{
    private readonly IDbReadOnlyEntityMethods<BankRegistrationEntity> _bankRegistrationMethods;
    private readonly IDbReadOnlyMethods _dbMethods;
    private readonly IDbReadOnlyEntityMethods<ExternalApiSecretEntity> _externalApiSecretMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbReadOnlyEntityMethods<SoftwareStatementEntity> _softwareStatementMethods;


    public ConsentCommon(
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        IInstrumentationClient instrumentationClient,
        IDbReadOnlyMethods dbMethods,
        IDbReadOnlyEntityMethods<ExternalApiSecretEntity> externalApiSecretMethods,
        IDbReadOnlyEntityMethods<SoftwareStatementEntity> softwareStatementMethods)
    {
        _bankRegistrationMethods = bankRegistrationMethods;
        _instrumentationClient = instrumentationClient;
        _dbMethods = dbMethods;
        _externalApiSecretMethods = externalApiSecretMethods;
        _softwareStatementMethods = softwareStatementMethods;
    }

    public async
        Task<(BankRegistrationEntity bankRegistration, string tokenEndpoint, SoftwareStatementEntity softwareStatement,
            ExternalApiSecretEntity? externalApiSecret)> GetBankRegistration(
            Guid bankRegistrationId)
    {
        // Load BankRegistration
        BankRegistrationEntity bankRegistration;
        SoftwareStatementEntity softwareStatement;
        ExternalApiSecretEntity? externalApiSecret;
        if (_dbMethods.DbProvider is not DbProvider.MongoDb)
        {
            bankRegistration =
                await _bankRegistrationMethods
                    .DbSetNoTracking
                    .Include(o => o.SoftwareStatementNavigation)
                    .Include(o => o.ExternalApiSecretsNavigation)
                    .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            softwareStatement = bankRegistration.SoftwareStatementNavigation;
            externalApiSecret = bankRegistration.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        }
        else
        {
            bankRegistration =
                await _bankRegistrationMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            softwareStatement = await _softwareStatementMethods
                .DbSetNoTracking
                .SingleAsync(x => x.Id == bankRegistration.SoftwareStatementId);
            externalApiSecret = await _externalApiSecretMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.BankRegistrationId == bankRegistration.Id && !x.IsDeleted);
        }
        string tokenEndpoint = bankRegistration.TokenEndpoint;

        return (bankRegistration, tokenEndpoint, softwareStatement, externalApiSecret);
    }
}
