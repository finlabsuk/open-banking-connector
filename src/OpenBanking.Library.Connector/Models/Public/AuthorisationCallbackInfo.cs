// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    public class AuthorisationCallbackInfo
    {
        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("code")]
        public string AuthorisationCode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("nonce", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Nonce { get; set; }
    }
}
