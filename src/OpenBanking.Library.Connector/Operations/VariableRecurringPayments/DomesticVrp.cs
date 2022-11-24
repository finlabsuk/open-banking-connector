// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments
{
    /// <summary>
    ///     Domestic VRP operations.
    /// </summary>
    internal class DomesticVrp :
        IExternalRead<DomesticVrpResponse>,
        IExternalCreate<DomesticVrpRequest, DomesticVrpResponse>
    {
        private readonly AuthContextAccessTokenGet _authContextAccessTokenGet;
        private readonly DomesticVrpConsentCommon _domesticVrpConsentCommon;
        private readonly IGrantPost _grantPost;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IApiVariantMapper _mapper;
        private readonly ITimeProvider _timeProvider;

        public DomesticVrp(
            IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
            IInstrumentationClient instrumentationClient,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IApiVariantMapper mapper,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost,
            AuthContextAccessTokenGet authContextAccessTokenGet)
        {
            _instrumentationClient = instrumentationClient;
            _mapper = mapper;
            _timeProvider = timeProvider;
            _grantPost = grantPost;
            _authContextAccessTokenGet = authContextAccessTokenGet;
            _domesticVrpConsentCommon = new DomesticVrpConsentCommon(
                entityMethods,
                instrumentationClient,
                softwareStatementProfileRepo);
        }

        private string ClientCredentialsGrantScope => "payments";

        private string RelativePathBeforeId => "/domestic-vrps";

        public async Task<(DomesticVrpResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            CreateAsync(DomesticVrpRequest request, Guid consentId, string? createdBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load DomesticVrpConsent and related
            (DomesticVrpConsentPersisted persistedConsent, string externalApiConsentId,
                    VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi, BankRegistration bankRegistration,
                    string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _domesticVrpConsentCommon.GetDomesticVrpConsent(consentId);

            // Get access token
            string bankIssuerUrl =
                persistedConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                    ?.DomesticVrpConsentAuthGet
                    ?.AudClaim ??
                bankRegistration.BankNavigation.IssuerUrl;
            string accessToken =
                await _authContextAccessTokenGet.GetAccessTokenAndUpdateConsent(
                    persistedConsent,
                    bankIssuerUrl,
                    "openid payments",
                    bankRegistration,
                    persistedConsent.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
                    createdBy);

            // Create external object at bank API
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiPostRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> apiRequests =
                ApiRequests(
                    variableRecurringPaymentsApi.ApiVersion,
                    bankFinancialId,
                    accessToken,
                    processedSoftwareStatementProfile);
            Uri externalApiUrl = new(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId);
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest externalApiRequest = request.ExternalApiRequest;
            if (string.IsNullOrEmpty(request.ExternalApiRequest.Data.ConsentId))
            {
                externalApiRequest.Data.ConsentId = externalApiConsentId;
            }
            else if (externalApiRequest.Data.ConsentId != externalApiConsentId)
            {
                throw new ArgumentException(
                    $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                    "inferred from specified DomesticVrpConsent.");
            }

            (VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.PostAsync(
                    externalApiUrl,
                    externalApiRequest,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            var response = new DomesticVrpResponse(externalApiResponse);
            return (response, nonErrorMessages);
        }

        public async Task<(DomesticVrpResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ReadAsync(string externalId, Guid consentId, string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load DomesticVrpConsent and related
            (DomesticVrpConsentPersisted persistedConsent, string _,
                    VariableRecurringPaymentsApiEntity variableRecurringPaymentsApi, BankRegistration bankRegistration,
                    string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
                await _domesticVrpConsentCommon.GetDomesticVrpConsent(consentId);

            // Get client credentials grant access token
            string ccGrantAccessToken =
                (await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    persistedConsent.BankRegistrationNavigation.BankNavigation.TokenEndpoint,
                    null,
                    processedSoftwareStatementProfile.ApiClient,
                    _instrumentationClient))
                .AccessToken;

            // Read object from external API
            JsonSerializerSettings? responseJsonSerializerSettings = null;
            IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> apiRequests =
                ApiRequests(
                    variableRecurringPaymentsApi.ApiVersion,
                    bankFinancialId,
                    ccGrantAccessToken,
                    processedSoftwareStatementProfile);
            Uri externalApiUrl = new(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId + $"/{externalId}");
            (VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse,
                    IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
                await apiRequests.GetAsync(
                    externalApiUrl,
                    responseJsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            var response = new DomesticVrpResponse(externalApiResponse);
            return (response, nonErrorMessages);
        }

        private
            IApiRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> ApiRequests(
                VariableRecurringPaymentsApiVersion variableRecurringPaymentsApiVersion,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =>
            variableRecurringPaymentsApiVersion switch
            {
                VariableRecurringPaymentsApiVersion.Version3p1p8 => new ApiRequests<
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest,
                    VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                    new ApiGetRequestProcessor(bankFinancialId, accessToken),
                    new PaymentInitiationPostRequestProcessor<
                        VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest>(
                        bankFinancialId,
                        accessToken,
                        _instrumentationClient,
                        false,
                        processedSoftwareStatementProfile)),
                _ => throw new ArgumentOutOfRangeException(
                    $"Variable Recurring Payments API version {variableRecurringPaymentsApiVersion} not supported.")
            };
    }
}
