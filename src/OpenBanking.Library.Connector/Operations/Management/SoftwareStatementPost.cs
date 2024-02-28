// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using ObSealCertificateCached = FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management.ObSealCertificate;
using ObWacCertificateCached = FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management.ObWacCertificate;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class SoftwareStatementPost(
    IDbReadWriteEntityMethods<SoftwareStatementEntity> entityMethods,
    IDbSaveChangesMethod dbSaveChangesMethod,
    ITimeProvider timeProvider,
    IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
    IInstrumentationClient instrumentationClient,
    ObWacCertificateMethods obWacCertificateMethods,
    ObSealCertificateMethods obSealCertificateMethods)
    : IObjectCreate<SoftwareStatement, SoftwareStatementResponse, LocalCreateParams>
{
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

        // Load related OBWAC
        ObWacCertificateCached processedTransportCertificateProfile =
            await obWacCertificateMethods.GetValue(entity.DefaultObWacCertificateId);

        // Load related OBSeal
        ObSealCertificateCached processedSigningCertificateProfile =
            await obSealCertificateMethods.GetValue(entity.DefaultObSealCertificateId);

        // Add software statement to cache
        var softwareStatementIdString = entity.Id.ToString();
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile = softwareStatementProfileRepo.GetProfile(
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
            instrumentationClient);

        softwareStatementProfileRepo.AddProfile(processedSoftwareStatementProfile, softwareStatementIdString);

        // Add entity
        await entityMethods.AddAsync(entity);

        // Create response
        SoftwareStatementResponse response = entity.PublicGetLocalResponse;

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }
}
