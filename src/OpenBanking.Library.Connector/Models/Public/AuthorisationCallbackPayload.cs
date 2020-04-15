// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    public class AuthorisationCallbackPayload
    {
        public AuthorisationCallbackPayload(string authorisationCode, string state)
        {
            AuthorisationCode = authorisationCode;
            State = state;
        }

        [JsonProperty("id_token")]
        public string IdToken { get; set; } = null;

        [JsonProperty("code")]
        public string AuthorisationCode { get; }

        [JsonProperty("state")]
        public string State { get; }

        [JsonProperty("nonce", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Nonce { get; set; } = null;
    }
}
