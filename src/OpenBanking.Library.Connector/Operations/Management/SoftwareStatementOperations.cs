// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class SoftwareStatementOperations(
    IDbEntityMethods<SoftwareStatementEntity> entityMethods,
    IDbMethods dbSaveChangesMethod,
    ITimeProvider timeProvider,
    IInstrumentationClient instrumentationClient)
    : IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams>,
        IObjectUpdate2<SoftwareStatementUpdate, SoftwareStatementResponse>,
        IObjectReadAll<SoftwareStatementsResponse, LocalReadAllParams>
{
    private readonly IInstrumentationClient _instrumentationClient = instrumentationClient;

    public async Task<(SoftwareStatementResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(
            SoftwareStatement request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create entity
        DateTimeOffset utcNow = timeProvider.GetUtcNow();
        var entity = new SoftwareStatementEntity(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.OrganisationId,
            request.SoftwareId,
            request.SandboxEnvironment,
            request.DefaultObWacCertificateId,
            request.DefaultObSealCertificateId,
            request.DefaultQueryRedirectUrl,
            request.DefaultFragmentRedirectUrl);

        // Add entity
        await entityMethods.AddAsync(entity);

        // Create response
        SoftwareStatementResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async Task<(SoftwareStatementsResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        ReadAllAsync(
            LocalReadAllParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();


        // Load entities
        List<SoftwareStatementResponseItem> items = await entityMethods
            .DbSetNoTracking
            .Select(
                x => new SoftwareStatementResponseItem
                {
                    OrganisationId = x.OrganisationId,
                    SoftwareId = x.SoftwareId,
                    SandboxEnvironment = x.SandboxEnvironment,
                    DefaultObWacCertificateId = x.DefaultObWacCertificateId,
                    DefaultObSealCertificateId = x.DefaultObSealCertificateId,
                    DefaultQueryRedirectUrl = x.DefaultQueryRedirectUrl,
                    DefaultFragmentRedirectUrl = x.DefaultFragmentRedirectUrl,
                    Id = x.Id,
                    Created = x.Created,
                    CreatedBy = x.CreatedBy,
                    Reference = x.Reference
                })
            .ToListAsync();

        var response = new SoftwareStatementsResponse
        {
            Data = items,
            Warnings = null
        };

        return (response, nonErrorMessages);
    }

    public async Task<(SoftwareStatementResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        UpdateAsync(
            SoftwareStatementUpdate request,
            LocalReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load BankRegistration
        SoftwareStatementEntity entity =
            await entityMethods
                .DbSet
                .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
            throw new KeyNotFoundException($"No record found for SoftwareStatement with ID {readParams.Id}.");

        // Exit if no changes
        if (request.DefaultFragmentRedirectUrl is null &&
            request.DefaultQueryRedirectUrl is null &&
            request.DefaultObSealCertificateId is null &&
            request.DefaultObWacCertificateId is null)
        {
            return (entity.PublicGetLocalResponse, nonErrorMessages);
        }

        // Update entity
        DateTimeOffset utcNow = timeProvider.GetUtcNow();
        entity.Update(
            request.DefaultFragmentRedirectUrl,
            request.DefaultQueryRedirectUrl,
            request.DefaultObSealCertificateId,
            request.DefaultObWacCertificateId,
            utcNow);

        // Create response
        SoftwareStatementResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
