// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Json;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    public class OpenBankingClientRegistrationResponse
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("client_id_issued_at")]
        [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
        public DateTimeOffset ClientIdIssuedAt { get; set; }

        [JsonProperty("client_secret_expires_at")]
        [JsonConverter(typeof(DateTimeOffsetUnixConverter))]
        public DateTimeOffset ClientSecretExpiresAt { get; set; }

        [JsonProperty("redirect_uris")]
        public string[] RedirectUris { get; set; }

        [JsonProperty("token_endpoint_auth_method")]
        public string TokenEndpointAuthMethod { get; set; }

        [JsonProperty("grant_types")]
        public string[] GrantTypes { get; set; }

        [JsonProperty("response_types")]
        public string[] ResponseTypes { get; set; }

        [JsonProperty("software_id")]
        public string SoftwareId { get; set; }

        [JsonProperty("scope")]
        public string[] Scope { get; set; }

        [JsonProperty("application_type")]
        public string ApplicationType { get; set; }

        [JsonProperty("id_token_signed_response_alg")]
        public string IdTokenSignedResponseAlg { get; set; }

        [JsonProperty("request_object_signing_alg")]
        public string RequestObjectSigningAlg { get; set; }

        [JsonProperty("token_endpoint_auth_signing_alg")]
        public string TokenEndpointAuthSigningAlg { get; set; }

        [JsonProperty("tls_client_auth_subject_dn")]
        public string TlsClientAuthSubjectDn { get; set; }
    }
}
