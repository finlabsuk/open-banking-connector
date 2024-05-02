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
    IDbReadWriteEntityMethods<SoftwareStatementEntity> entityMethods,
    IDbSaveChangesMethod dbSaveChangesMethod,
    ITimeProvider timeProvider,
    IInstrumentationClient instrumentationClient)
    : IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams>,
        IObjectUpdate2<SoftwareStatementUpdate, SoftwareStatementResponse>
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
            request.CreatedBy)
        {
            OrganisationId = request.OrganisationId,
            SoftwareId = request.SoftwareId,
            SandboxEnvironment = request.SandboxEnvironment,
            DefaultObWacCertificateId = request.DefaultObWacCertificateId,
            DefaultObSealCertificateId = request.DefaultObSealCertificateId,
            DefaultQueryRedirectUrl = request.DefaultQueryRedirectUrl,
            DefaultFragmentRedirectUrl = request.DefaultFragmentRedirectUrl,
            Modified = utcNow
        };

        // Add entity
        await entityMethods.AddAsync(entity);

        // Create response
        SoftwareStatementResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await dbSaveChangesMethod.SaveChangesAsync();

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
        if (request.DefaultFragmentRedirectUrl is not null)
        {
            entity.DefaultFragmentRedirectUrl = request.DefaultFragmentRedirectUrl;
        }
        if (request.DefaultQueryRedirectUrl is not null)
        {
            entity.DefaultQueryRedirectUrl = request.DefaultQueryRedirectUrl;
        }
        if (request.DefaultObSealCertificateId is not null)
        {
            entity.DefaultObSealCertificateId = request.DefaultObSealCertificateId.Value;
        }
        if (request.DefaultObWacCertificateId is not null)
        {
            entity.DefaultObWacCertificateId = request.DefaultObWacCertificateId.Value;
        }
        DateTimeOffset utcNow = timeProvider.GetUtcNow();
        entity.Modified = utcNow;

        // Create response
        SoftwareStatementResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
