// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public
{
    public class OAuth2RequestObjectClaims
    {
        [JsonProperty("iss")]
        public string Iss { get; set; }

        [JsonProperty("aud")]
        public string Aud { get; set; }

        [JsonProperty("jti")]
        public string Jti { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("max_age")]
        public int MaxAge { get; set; }

        [JsonProperty("claims")]
        public OAuth2RequestObjectInnerClaims Claims { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("state")]
        public string State { get; set; } = Guid.NewGuid().ToString();
    }
}
