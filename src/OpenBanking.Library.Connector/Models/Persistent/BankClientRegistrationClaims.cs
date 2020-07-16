// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    public class BankClientRegistrationClaims
    {
        [JsonProperty("iss")]
        public string Iss { get; set; }

        [JsonProperty("iat")]
        public DateTimeOffset Iat { get; set; } = DateTimeOffset.UtcNow;

        [JsonProperty("exp")]
        public DateTimeOffset Exp { get; set; } = DateTimeOffset.UtcNow.AddHours(1);

        [JsonProperty("jti")]
        public string Jti { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [JsonProperty("token_endpoint_auth_method")]
        public string TokenEndpointAuthMethod { get; set; } = "tls_client_auth";

        [JsonProperty("grant_types")]
        public string[] GrantTypes { get; set; } = { "client_credentials", "authorization_code", "refresh_token" };

        [JsonProperty("response_types")]
        public string[] ResponseTypes { get; set; } = { "code id_token" };

        [JsonProperty("application_type")]
        public string ApplicationType { get; set; } = "web";

        [JsonProperty("token_endpoint_auth_signing_alg")]
        public string? TokenEndpointAuthSigningAlg { get; set; }

        [JsonProperty("aud")]
        public string Aud { get; set; }

        [JsonProperty("id_token_signed_response_alg")]
        public string IdTokenSignedResponseAlg { get; set; } = "PS256";

        [JsonProperty("redirect_uris")]
        public string[] RedirectUris { get; set; }

        [JsonProperty("request_object_signing_alg")]
        public string RequestObjectSigningAlg { get; set; } = "PS256";

        //TODO: this is ordinarily a single string...
        [JsonProperty("scope")]
        public string[] Scope { get; set; }

        [JsonProperty("software_id")]
        public string SoftwareId { get; set; }

        [JsonProperty("software_statement")]
        public string SoftwareStatement { get; set; }

        [JsonProperty("tls_client_auth_subject_dn")]
        public string TlsClientAuthSubjectDn { get; set; }

        // NB subject_type removed
    }
}
