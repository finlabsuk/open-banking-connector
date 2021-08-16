// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal static class PostTokenRequest
    {
        public static async Task<TokenEndpointResponse> PostClientCredentialsGrantAsync(
            string? scope,
            BankRegistration bankRegistration,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient apiClient)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            if (!(scope is null))
            {
                keyValuePairs["scope"] = scope;
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
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
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
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient apiClient)
        {
            // POST request
            Uri uri = new Uri(bankRegistration.OpenIdConfiguration.TokenEndpoint);
            IPostRequestProcessor<Dictionary<string, string>> postRequestProcessor =
                new AuthGrantPostRequestProcessor<Dictionary<string, string>>(bankRegistration);
            var response = await postRequestProcessor.PostAsync<TokenEndpointResponse>(
                uri,
                keyValuePairs,
                jsonSerializerSettings,
                apiClient);

            // TODO: validate response?

            return response;
        }
    }
}
