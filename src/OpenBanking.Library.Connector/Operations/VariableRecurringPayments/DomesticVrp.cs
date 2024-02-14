// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
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

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

/// <summary>
///     Domestic VRP operations.
/// </summary>
internal class DomesticVrp :
    IExternalRead<DomesticVrpResponse>,
    IExternalCreate<DomesticVrpRequest, DomesticVrpResponse>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly ConsentAccessTokenGet _consentAccessTokenGet;
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
        ConsentAccessTokenGet consentAccessTokenGet,
        IBankProfileService bankProfileService)
    {
        _instrumentationClient = instrumentationClient;
        _mapper = mapper;
        _timeProvider = timeProvider;
        _grantPost = grantPost;
        _consentAccessTokenGet = consentAccessTokenGet;
        _bankProfileService = bankProfileService;
        _domesticVrpConsentCommon = new DomesticVrpConsentCommon(
            entityMethods,
            instrumentationClient,
            softwareStatementProfileRepo);
    }

    private string ClientCredentialsGrantScope => "payments";

    private string RelativePathBeforeId => "/domestic-vrps";

    public async Task<(DomesticVrpResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(DomesticVrpRequest request)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
                DomesticVrpConsentAccessToken? storedAccessToken, DomesticVrpConsentRefreshToken? storedRefreshToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(request.DomesticVrpConsentId, true);
        string externalApiConsentId = persistedConsent.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(
            bankRegistration.BankProfile,
            _instrumentationClient);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        string issuerUrl = bankProfile.IssuerUrl;
        string bankFinancialId = bankProfile.FinancialId;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;

        // Get access token
        string bankTokenIssuerClaim = DomesticVrpConsentCommon.GetBankTokenIssuerClaim(
            customBehaviour,
            issuerUrl); // Get bank token issuer ("iss") claim
        string accessToken =
            await _consentAccessTokenGet.GetAccessTokenAndUpdateConsent(
                persistedConsent,
                bankTokenIssuerClaim,
                "openid payments",
                bankRegistration,
                storedAccessToken,
                storedRefreshToken,
                tokenEndpointAuthMethod,
                persistedConsent.BankRegistrationNavigation.TokenEndpoint,
                supportsSca,
                idTokenSubClaimType,
                customBehaviour?.RefreshTokenGrantPost,
                customBehaviour?.JwksGet,
                request.ModifiedBy);

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
        var externalApiUrl = new Uri(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId);
        if (string.IsNullOrEmpty(request.ExternalApiRequest.Data.ConsentId))
        {
            request.ExternalApiRequest.Data.ConsentId = externalApiConsentId;
        }
        else if (request.ExternalApiRequest.Data.ConsentId != externalApiConsentId)
        {
            throw new ArgumentException(
                $"ExternalApiRequest contains consent ID that differs from {externalApiConsentId} " +
                "inferred from specified DomesticVrpConsent.");
        }

        (VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.PostAsync(
                externalApiUrl,
                request.ExternalApiRequest,
                requestJsonSerializerSettings,
                responseJsonSerializerSettings,
                processedSoftwareStatementProfile.ApiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Create response
        var response = new DomesticVrpResponse { ExternalApiResponse = externalApiResponse };
        return (response, nonErrorMessages);
    }

    public async Task<(DomesticVrpResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(string externalId, Guid consentId, string? modifiedBy)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (DomesticVrpConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration, _, _,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(consentId, false);

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(
            bankRegistration.BankProfile,
            _instrumentationClient);
        VariableRecurringPaymentsApi variableRecurringPaymentsApi =
            bankProfile.GetRequiredVariableRecurringPaymentsApi();
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        bool supportsSca = bankProfile.SupportsSca;
        string bankFinancialId = bankProfile.FinancialId;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

        // Get client credentials grant access token
        string ccGrantAccessToken =
            await _grantPost.PostClientCredentialsGrantAsync(
                ClientCredentialsGrantScope,
                processedSoftwareStatementProfile.OBSealKey,
                tokenEndpointAuthMethod,
                persistedConsent.BankRegistrationNavigation.TokenEndpoint,
                persistedConsent.BankRegistrationNavigation.ExternalApiId,
                persistedConsent.BankRegistrationNavigation.ExternalApiSecret,
                persistedConsent.BankRegistrationNavigation.Id.ToString(),
                null,
                customBehaviour?.ClientCredentialsGrantPost,
                processedSoftwareStatementProfile.ApiClient);

        // Read object from external API
        JsonSerializerSettings? responseJsonSerializerSettings = null;
        IApiGetRequests<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> apiRequests =
            ApiRequests(
                variableRecurringPaymentsApi.ApiVersion,
                bankFinancialId,
                ccGrantAccessToken,
                processedSoftwareStatementProfile);
        var externalApiUrl =
            new Uri(variableRecurringPaymentsApi.BaseUrl + RelativePathBeforeId + $"/{externalId}");
        (VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse externalApiResponse,
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await apiRequests.GetAsync(
                externalApiUrl,
                responseJsonSerializerSettings,
                processedSoftwareStatementProfile.ApiClient,
                _mapper);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Create response
        var response = new DomesticVrpResponse { ExternalApiResponse = externalApiResponse };
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
            VariableRecurringPaymentsApiVersion.VersionPublic => new ApiRequests<
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
                    processedSoftwareStatementProfile)),
            _ => throw new ArgumentOutOfRangeException(
                $"Variable Recurring Payments API version {variableRecurringPaymentsApiVersion} not supported.")
        };
}
