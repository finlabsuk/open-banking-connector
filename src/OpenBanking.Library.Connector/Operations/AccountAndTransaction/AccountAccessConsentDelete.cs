// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class AccountAccessConsentDelete : BaseDelete<AccountAccessConsent, ConsentDeleteParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly IGrantPost _grantPost;

    public AccountAccessConsentDelete(
        IDbReadWriteEntityMethods<AccountAccessConsent> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IGrantPost grantPost,
        IBankProfileService bankProfileService) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient)
    {
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
    }

    protected string RelativePathBeforeId => "/account-access-consents";

    protected string RelativePathAfterId => "";

    protected override async
        Task<(AccountAccessConsent persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ApiDelete(ConsentDeleteParams deleteParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load object
        AccountAccessConsent persistedObject =
            await _entityMethods
                .DbSet
                .Include(o => o.BankRegistrationNavigation)
                .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
            throw new KeyNotFoundException($"No record found for Account Access Consent with ID {deleteParams.Id}.");

        if (deleteParams.IncludeExternalApiOperation)
        {
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankApiId = persistedObject.ExternalApiId;

            // Get bank profile
            BankProfile bankProfile =
                _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            AccountAndTransactionApi accountAndTransactionApi =
                bankProfile.GetRequiredAccountAndTransactionApi();
            TokenEndpointAuthMethod tokenEndpointAuthMethod =
                bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
            bool supportsSca = bankProfile.SupportsSca;
            string bankFinancialId = bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Determine endpoint URL
            string baseUrl = accountAndTransactionApi.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            // Get client credentials grant token
            string ccGrantAccessToken =
                await _grantPost.PostClientCredentialsGrantAsync(
                    "accounts",
                    processedSoftwareStatementProfile.OBSealKey,
                    bankRegistration,
                    tokenEndpointAuthMethod,
                    persistedObject.BankRegistrationNavigation.TokenEndpoint,
                    supportsSca,
                    null,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient);
            IDeleteRequestProcessor deleteRequestProcessor =
                new ApiDeleteRequestProcessor(ccGrantAccessToken, bankFinancialId);

            // Delete at API
            await deleteRequestProcessor.DeleteAsync(endpointUrl, apiClient);
        }

        return (persistedObject, nonErrorMessages);
    }
}
