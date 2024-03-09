// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class BankRegistrationDelete : BaseDelete<BankRegistrationEntity, BankRegistrationDeleteParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly IGrantPost _grantPost;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;

    public BankRegistrationDelete(
        IDbReadWriteEntityMethods<BankRegistrationEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        IGrantPost grantPost,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        instrumentationClient)
    {
        _bankProfileService = bankProfileService;
        _grantPost = grantPost;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
    }

    protected override async
        Task<(BankRegistrationEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ApiDelete(BankRegistrationDeleteParams deleteParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load BankRegistration
        BankRegistrationEntity entity =
            await _entityMethods
                .DbSet
                .Include(o => o.SoftwareStatementNavigation)
                .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
            throw new KeyNotFoundException($"No record found for Bank Registration with ID {deleteParams.Id}.");
        SoftwareStatementEntity softwareStatement = entity.SoftwareStatementNavigation!;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(entity.BankProfile);
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
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
            // Get IApiClient
            IApiClient apiClient = entity.UseSimulatedBank
                ? bankProfile.ReplayApiClient
                : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

            // Get OBSeal key
            OBSealKey obSealKey =
                (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

            // Get URI
            string bankApiId = entity.ExternalApiId;
            var apiRequestUrl = new Uri(registrationEndpoint.TrimEnd('/') + $"/{bankApiId}");

            // Get appropriate token
            string accessToken;
            if (useRegistrationAccessTokenValue)
            {
                accessToken =
                    entity.RegistrationAccessToken ??
                    throw new InvalidOperationException("No registration access token available");
            }
            else
            {
                string? scope = customBehaviour?.BankRegistrationPut?.CustomTokenScope;
                accessToken = await _grantPost.PostClientCredentialsGrantAsync(
                    scope,
                    obSealKey,
                    tokenEndpointAuthMethod,
                    entity.TokenEndpoint,
                    entity.ExternalApiId,
                    entity.ExternalApiSecret,
                    entity.Id.ToString(),
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient,
                    bankProfile.BankProfileEnum);
            }

            // Delete at API
            IDeleteRequestProcessor deleteRequestProcessor =
                new BankRegistrationDeleteRequestProcessor(accessToken);
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription = "DELETE {RegistrationEndpoint}/{ClientId}",
                BankProfile = bankProfile.BankProfileEnum
            };
            await deleteRequestProcessor.DeleteAsync(apiRequestUrl, tppReportingRequestInfo, apiClient);
        }

        return (entity, nonErrorMessages);
    }
}
