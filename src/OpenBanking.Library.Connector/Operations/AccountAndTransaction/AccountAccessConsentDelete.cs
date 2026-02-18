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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class AccountAccessConsentDelete : BaseDelete<AccountAccessConsent, ConsentDeleteParams>
{
    private readonly AccountAccessConsentCommon _accountAccessConsentCommon;
    private readonly IBankProfileService _bankProfileService;
    private readonly ClientAccessTokenGet _clientAccessTokenGet;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ObWacCertificateMethods _obWacCertificateMethods;

    public AccountAccessConsentDelete(
        IDbEntityMethods<AccountAccessConsent> entityMethods,
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObSealCertificateMethods obSealCertificateMethods,
        ObWacCertificateMethods obWacCertificateMethods,
        ClientAccessTokenGet clientAccessTokenGet,
        AccountAccessConsentCommon accountAccessConsentCommon) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        instrumentationClient)
    {
        _bankProfileService = bankProfileService;
        _obSealCertificateMethods = obSealCertificateMethods;
        _obWacCertificateMethods = obWacCertificateMethods;
        _clientAccessTokenGet = clientAccessTokenGet;
        _accountAccessConsentCommon = accountAccessConsentCommon;
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
        (AccountAccessConsent persistedConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret) =
            await _accountAccessConsentCommon.GetAccountAccessConsent(deleteParams.Id, true);
        if (!deleteParams.ExcludeExternalApiOperation)
        {
            string bankApiId = persistedConsent.ExternalApiId;

            // Get bank profile
            BankProfile bankProfile =
                _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
            bool aispUseV4 = bankRegistration.AispUseV4;
            AccountAndTransactionApi accountAndTransactionApi =
                bankProfile.GetRequiredAccountAndTransactionApi(aispUseV4);
            string bankFinancialId =
                bankProfile.AccountAndTransactionApiSettings.GetFinancialId?.Invoke(aispUseV4) ??
                bankProfile.FinancialId;
            CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

            // Get IApiClient
            // IApiClient apiClient = bankRegistration.UseSimulatedBank
            //     ? bankProfile.ReplayApiClient
            //     : (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;
            IApiClient apiClient =
                (await _obWacCertificateMethods.GetValue(softwareStatement.DefaultObWacCertificateId)).ApiClient;

            // Get OBSeal key
            OBSealKey obSealKey =
                (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

            // Determine endpoint URL
            string baseUrl = accountAndTransactionApi.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            // Get client credentials grant token
            string ccGrantAccessToken =
                await _clientAccessTokenGet.GetAccessToken(
                    "accounts",
                    obSealKey,
                    bankRegistration,
                    externalApiSecret,
                    customBehaviour?.ClientCredentialsGrantPost,
                    apiClient,
                    bankProfile.BankProfileEnum);
            IDeleteRequestProcessor deleteRequestProcessor =
                new ApiDeleteRequestProcessor(ccGrantAccessToken, bankFinancialId);

            // Delete at API
            var tppReportingRequestInfo = new TppReportingRequestInfo
            {
                EndpointDescription =
                    $$"""
                      DELETE {AispBaseUrl}{{RelativePathBeforeId}}/{ConsentId}
                      """,
                BankProfile = bankProfile.BankProfileEnum
            };
            await deleteRequestProcessor.DeleteAsync(
                endpointUrl,
                deleteParams.ExtraHeaders,
                tppReportingRequestInfo,
                apiClient);
        }

        return (persistedConsent, nonErrorMessages);
    }
}
