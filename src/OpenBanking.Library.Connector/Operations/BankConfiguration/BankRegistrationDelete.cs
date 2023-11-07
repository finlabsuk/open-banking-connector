// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;

internal class BankRegistrationDelete : BaseDelete<BankRegistration, BankRegistrationDeleteParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly IGrantPost _grantPost;

    public BankRegistrationDelete(
        IDbReadWriteEntityMethods<BankRegistration> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        IGrantPost grantPost) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient)
    {
        _bankProfileService = bankProfileService;
        _grantPost = grantPost;
    }

    protected override async
        Task<(BankRegistration persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ApiDelete(BankRegistrationDeleteParams deleteParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load BankRegistration
        BankRegistration entity =
            await _entityMethods
                .DbSet
                .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
            throw new KeyNotFoundException($"No record found for Bank Registration with ID {deleteParams.Id}.");

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(entity.BankProfile);
        TokenEndpointAuthMethod tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

        bool includeExternalApiOperationValue =
            deleteParams.IncludeExternalApiOperation ??
            bankProfile.BankConfigurationApiSettings.UseRegistrationDeleteEndpoint;
        if (includeExternalApiOperationValue)
        {
            string registrationEndpoint =
                entity.RegistrationEndpoint ??
                throw new InvalidOperationException(
                    "BankRegistration does not have a registration endpoint configured.");

            bool useRegistrationAccessTokenValue =
                bankProfile.BankConfigurationApiSettings.UseRegistrationAccessToken;

            // Get API client
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    entity.SoftwareStatementProfileId,
                    entity.SoftwareStatementProfileOverride);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get URI
            string bankApiId = entity.ExternalApiObject.ExternalApiId;
            var apiRequestUrl = new Uri(registrationEndpoint.TrimEnd('/') + $"/{bankApiId}");

            // Get appropriate token
            string accessToken;
            if (useRegistrationAccessTokenValue)
            {
                accessToken =
                    entity.ExternalApiObject.RegistrationAccessToken ??
                    throw new InvalidOperationException("No registration access token available");
            }
            else
            {
                string? scope = customBehaviour?.BankRegistrationPut?.CustomTokenScope;
                accessToken = await _grantPost.PostClientCredentialsGrantAsync(
                    scope,
                    processedSoftwareStatementProfile.OBSealKey,
                    tokenEndpointAuthMethod,
                    entity.TokenEndpoint,
                    entity.ExternalApiObject.ExternalApiId,
                    entity.ExternalApiObject.ExternalApiSecret,
                    entity.Id.ToString(),
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient);
            }

            // Delete at API
            IDeleteRequestProcessor deleteRequestProcessor =
                new BankRegistrationDeleteRequestProcessor(accessToken);
            await deleteRequestProcessor.DeleteAsync(apiRequestUrl, apiClient);
        }

        return (entity, nonErrorMessages);
    }
}
