// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
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
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly IEncryptionKeyDescription _encryptionKeyInfo;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;

    public BankRegistrationDelete(
        IDbReadWriteEntityMethods<BankRegistrationEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObWacCertificateMethods obWacCertificateMethods,
        ObSealCertificateMethods obSealCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        IEncryptionKeyDescription encryptionKeyInfo) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        instrumentationClient)
    {
        _bankProfileService = bankProfileService;
        _obWacCertificateMethods = obWacCertificateMethods;
        _obSealCertificateMethods = obSealCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _encryptionKeyInfo = encryptionKeyInfo;
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
                .Include(o => o.ExternalApiSecretsNavigation)
                .Include(o => o.RegistrationAccessTokensNavigation)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
            throw new KeyNotFoundException($"No record found for Bank Registration with ID {deleteParams.Id}.");
        SoftwareStatementEntity softwareStatement = entity.SoftwareStatementNavigation!;
        ExternalApiSecretEntity? externalApiSecret =
            entity.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        RegistrationAccessTokenEntity? registrationAccessTokenEntity = entity.RegistrationAccessTokensNavigation
            .SingleOrDefault(x => !x.IsDeleted);

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(entity.BankProfile);
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

        bool excludeExternalApiOperation =
            deleteParams.ExcludeExternalApiOperation ||
            !bankProfile.BankConfigurationApiSettings.UseRegistrationDeleteEndpoint;
        if (!excludeExternalApiOperation)
        {
            string registrationEndpoint =
                entity.RegistrationEndpoint ??
                throw new InvalidOperationException(
                    "BankRegistration does not have a registration endpoint configured.");

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
            bool useRegistrationAccessToken =
                bankProfile.BankConfigurationApiSettings.UseRegistrationAccessToken;
            if (useRegistrationAccessToken)
            {
                if (registrationAccessTokenEntity is null)
                {
                    throw new InvalidOperationException("No registration access token is available.");
                }

                // Extract registration access token
                accessToken = registrationAccessTokenEntity
                    .GetRegistrationAccessToken(
                        entity.GetAssociatedData(),
                        await _encryptionKeyInfo.GetEncryptionKey(
                            registrationAccessTokenEntity.EncryptionKeyDescriptionId));
            }
            else
            {
                string? scope = null;
                if (customBehaviour?.BankRegistrationPut?.GetCustomTokenScope is not null)
                {
                    scope = customBehaviour.BankRegistrationPut.GetCustomTokenScope(entity.RegistrationScope);
                }
                accessToken =
                    await _clientAccessTokenGet.GetAccessToken(
                        scope,
                        obSealKey,
                        entity,
                        externalApiSecret,
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
            await deleteRequestProcessor.DeleteAsync(
                apiRequestUrl,
                null,
                tppReportingRequestInfo,
                apiClient);
        }

        return (entity, nonErrorMessages);
    }
}
