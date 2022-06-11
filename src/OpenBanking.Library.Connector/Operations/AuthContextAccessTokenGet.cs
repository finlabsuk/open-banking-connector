// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;

        public AuthContextAccessTokenGet(
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IGrantPost grantPost)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _grantPost = grantPost;
        }

        public async Task<string> GetAccessTokenAndUpdateConsent<TConsentEntity>(
            TConsentEntity consent,
            string bankIssuerUrl,
            BankRegistration bankRegistration,
            string? modifiedBy)
            where TConsentEntity : BaseConsent
        {
            // Get token
            AccessToken accessToken =
                consent.AccessToken ??
                throw new InvalidOperationException("No access token is available for Consent.");
            string nonce =
                consent.Nonce ??
                throw new InvalidOperationException("No nonce is available for Consent.");
            string externalApiClientId = bankRegistration.ExternalApiObject.ExternalApiId;

            // Calculate token expiry time
            const int tokenEarlyExpiryIntervalInSeconds = 10;
            DateTimeOffset tokenExpiryTime = accessToken.Modified // time when token stored
                .AddSeconds(accessToken.ExpiresIn) // plus token duration ("expires_in")
                .AddSeconds(
                    -tokenEarlyExpiryIntervalInSeconds); // less margin to allow for time required to obtain token and to re-use token

            // If token expired, attempt to get a new one 
            if (_timeProvider.GetUtcNow() > tokenExpiryTime)
            {
                if (!(accessToken.RefreshToken is null))
                {
                    ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                        await _softwareStatementProfileRepo.GetAsync(
                            bankRegistration.SoftwareStatementProfileId,
                            bankRegistration.SoftwareStatementProfileOverride);

                    // Obtain token for consent
                    string redirectUrl =
                        processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;
                    JsonSerializerSettings? jsonSerializerSettings = null;
                    RefreshTokenGrantResponse tokenEndpointResponse =
                        await _grantPost.PostRefreshTokenGrantAsync(
                            accessToken.RefreshToken,
                            redirectUrl,
                            bankIssuerUrl,
                            externalApiClientId,
                            consent.ExternalApiId,
                            nonce,
                            bankRegistration,
                            jsonSerializerSettings,
                            processedSoftwareStatementProfile.ApiClient);

                    // Check token endpoint response
                    bool isBearerTokenType = string.Equals(
                        tokenEndpointResponse.TokenType,
                        "bearer",
                        StringComparison.OrdinalIgnoreCase);
                    if (!isBearerTokenType)
                    {
                        throw new InvalidDataException(
                            "Access token received does not have token type equal to Bearer or bearer.");
                    }

                    // Update auth context with token
                    consent.UpdateAccessToken(
                        tokenEndpointResponse.AccessToken,
                        tokenEndpointResponse.ExpiresIn,
                        tokenEndpointResponse.RefreshToken,
                        _timeProvider.GetUtcNow(),
                        modifiedBy);

                    // Persist updates (this happens last so as not to happen if there are any previous errors)
                    await _dbSaveChangesMethod.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException("Access token has expired and no refresh token is available.");
                }
            }

            return accessToken.Token;
        }
    }
}
