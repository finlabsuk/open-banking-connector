// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
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
        private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;

        public AuthContextAccessTokenGet(
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IDbSaveChangesMethod dbSaveChangesMethod)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _dbSaveChangesMethod = dbSaveChangesMethod;
        }

        public async Task<string> GetAccessToken<TAuthContext>(
            IList<TAuthContext> input,
            BankRegistration bankRegistration,
            string? modifiedBy)
            where TAuthContext : AuthContext
        {
            // Get token
            List<TAuthContext> authContextsWithToken =
                input
                    .Where(x => x.AccessToken.Value1 != null)
                    .ToList();

            if (!authContextsWithToken.Any())
            {
                throw new InvalidOperationException("No token is available for Consent.");
            }

            TAuthContext authContext = authContextsWithToken
                .OrderByDescending(x => x.AccessToken.Modified)
                .First();

            const int tokenEarlyExpiryIntervalInSeconds = 10;
            DateTimeOffset tokenExpiryTime = authContext.AccessToken.Modified // time when token stored
                .AddSeconds(authContext.AccessToken.Value2) // plus token duration ("expires_in")
                .AddSeconds(
                    -tokenEarlyExpiryIntervalInSeconds); // less margin to allow for time required to obtain token and to re-use token

            // If token expired, attempt to get a new one 
            if (tokenExpiryTime <= DateTimeOffset.UtcNow)
            {
                if (!(authContext.RefreshToken.Value is null))
                {
                    ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                        await _softwareStatementProfileRepo.GetAsync(
                            bankRegistration.SoftwareStatementProfileId,
                            bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);

                    // Obtain token for consent
                    string redirectUrl = processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;
                    JsonSerializerSettings? jsonSerializerSettings = null;
                    TokenEndpointResponse tokenEndpointResponse =
                        await PostTokenRequest.PostRefreshTokenGrantAsync(
                            authContext.RefreshToken.Value,
                            redirectUrl,
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
                    authContext.AccessToken = new ReadWritePropertyGroup<string?, int>(
                        tokenEndpointResponse.AccessToken,
                        tokenEndpointResponse.ExpiresIn,
                        new TimeProvider(),
                        modifiedBy);
                    authContext.RefreshToken = new ReadWriteProperty<string?>(
                        tokenEndpointResponse.RefreshToken,
                        new TimeProvider(),
                        modifiedBy);

                    // Persist updates (this happens last so as not to happen if there are any previous errors)
                    await _dbSaveChangesMethod.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException("Access token has expired and no refresh token is available.");
                }
            }

            return authContext.AccessToken.Value1!; // We already filtered out null entries above
        }
    }
}
