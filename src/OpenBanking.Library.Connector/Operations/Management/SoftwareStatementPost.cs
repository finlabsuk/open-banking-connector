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

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class SoftwareStatementPost(
    IDbReadWriteEntityMethods<SoftwareStatementEntity> entityMethods,
    IDbSaveChangesMethod dbSaveChangesMethod,
    ITimeProvider timeProvider,
    IInstrumentationClient instrumentationClient)
    : IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams>
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
}
