// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class AuthContextAccessTokenGet
    {
        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IGrantPost _grantPost;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public AuthContextAccessTokenGet(
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost,
            IInstrumentationClient instrumentationClient)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _grantPost = grantPost;
            _instrumentationClient = instrumentationClient;
        }

        public async Task<string> GetAccessTokenAndUpdateConsent<TConsentEntity>(
            TConsentEntity consent,
            string bankIssuerUrl,
            string? requestScope,
            BankRegistration bankRegistration,
            string tokenEndpoint,
            string? modifiedBy)
            where TConsentEntity : BaseConsent
        {
            // Get token
            AccessToken accessToken =
                consent.AccessToken ??
                throw new InvalidOperationException("No access token is available for Consent.");
            string nonce =
                consent.AuthContextNonce ??
                throw new InvalidOperationException("No nonce is available for Consent.");
            string externalApiClientId = bankRegistration.ExternalApiObject.ExternalApiId;

            // Calculate token expiry time
            const int tokenEarlyExpiryIntervalInSeconds = 10;
            DateTimeOffset tokenExpiryTime = accessToken.Modified // time when token stored
                .AddSeconds(accessToken.ExpiresIn) // plus token duration ("expires_in")
                .AddSeconds(
                    -tokenEarlyExpiryIntervalInSeconds); // less margin to allow for time required to obtain token and to re-use token

            // Return unexpired access token if available
            if (_timeProvider.GetUtcNow() <= tokenExpiryTime)
            {
                return accessToken.Token;
            }

            // Check that refresh token is available
            if (accessToken.RefreshToken is null)
            {
                throw new InvalidOperationException("Access token has expired and no refresh token is available.");
            }

            // Get new access token using refresh token 
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);

            // Obtain token for consent
            string redirectUrl = bankRegistration.DefaultRedirectUri;
            JsonSerializerSettings? jsonSerializerSettings = null;
            RefreshTokenGrantResponse tokenEndpointResponse =
                await _grantPost.PostRefreshTokenGrantAsync(
                    accessToken.RefreshToken,
                    redirectUrl,
                    bankIssuerUrl,
                    externalApiClientId,
                    consent.ExternalApiId,
                    nonce,
                    requestScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    tokenEndpoint,
                    jsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient,
                    _instrumentationClient);

            // Update auth context with token
            consent.UpdateAccessToken(
                tokenEndpointResponse.AccessToken,
                tokenEndpointResponse.ExpiresIn,
                tokenEndpointResponse.RefreshToken,
                _timeProvider.GetUtcNow(),
                modifiedBy);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return tokenEndpointResponse.AccessToken;
        }
    }
}
