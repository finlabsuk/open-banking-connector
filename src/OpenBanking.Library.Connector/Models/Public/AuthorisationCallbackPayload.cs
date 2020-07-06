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
            Code = authorisationCode;
            State = state;
        }

        public AuthorisationCallbackPayload() { }

        [JsonProperty("id_token")]
        public string Id_Token { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("nonce", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Nonce { get; set; }
    }
}
