// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal static class PostTokenRequest
    {
        public static async Task<TokenEndpointResponse> PostClientCredentialsGrantAsync(
            string? scope,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            BankRegistration bankRegistration,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient apiClient,
            IInstrumentationClient instrumentationClient)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            if (!(scope is null))
            {
                keyValuePairs["scope"] = scope;
            }

            if (bankRegistration.BankApiResponse.Data.TokenEndpointAuthMethod is
                OBRegistrationProperties1tokenEndpointAuthMethodEnum.PrivateKeyJwt)
            {
                // Create JWT
                var claims = new
                {
                    Iss = bankRegistration.BankApiResponse.Data.ClientId,
                    Sub = bankRegistration.BankApiResponse.Data.ClientId,
                    Aud = bankRegistration.OpenIdConfiguration.TokenEndpoint,
                    Jti = Guid.NewGuid().ToString(),
                    Iat = DateTimeOffset.Now,
                    Exp = DateTimeOffset.UtcNow.AddHours(1),
                };
                string jwt = JwtFactory.CreateJwt(
                    JwtFactory.DefaultJwtHeadersExcludingTyp(processedSoftwareStatementProfile.SigningKeyId),
                    claims,
                    processedSoftwareStatementProfile.SigningKey,
                    processedSoftwareStatementProfile.SigningCertificate);
                StringBuilder requestTraceSb = new StringBuilder()
                    .AppendLine("#### JWT (Client Auth)")
                    .Append(jwt);
                instrumentationClient.Info(requestTraceSb.ToString());

                // Add parameters
                keyValuePairs["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                keyValuePairs["client_assertion"] = jwt;
            }

            return await PostGrantAsync(
                keyValuePairs,
                bankRegistration,
                jsonSerializerSettings,
                apiClient);
        }

        public static async Task<TokenEndpointResponse> PostAuthCodeGrantAsync(
            string authCode,
            string redirectUrl,
            BankRegistration bankRegistration,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient apiClient)
        {
            var keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "code", authCode }
            };

            return await PostGrantAsync(
                keyValuePairs,
                bankRegistration,
                jsonSerializerSettings,
                apiClient);
        }

        private static async Task<TokenEndpointResponse> PostGrantAsync(
            Dictionary<string, string> keyValuePairs,
            BankRegistration bankRegistration,
            JsonSerializerSettings? responseJsonSerializerSettings,
            IApiClient apiClient)
        {
            // POST request
            var uri = new Uri(bankRegistration.OpenIdConfiguration.TokenEndpoint);
            IPostRequestProcessor<Dictionary<string, string>> postRequestProcessor =
                new AuthGrantPostRequestProcessor<Dictionary<string, string>>(bankRegistration);
            var response = await postRequestProcessor.PostAsync<TokenEndpointResponse>(
                uri,
                keyValuePairs,
                null,
                responseJsonSerializerSettings,
                apiClient);

            // TODO: validate response?

            return response;
        }
    }
}
