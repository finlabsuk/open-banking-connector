// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class AuthorisationCallbackData
    {
        public AuthorisationCallbackData(string responseMode, AuthorisationCallbackPayload response)
        {
            ResponseMode = responseMode;
            Response = response;
        }

        [JsonProperty("responseMode")]
        public string ResponseMode { get; }

        [JsonProperty("response")]
        public AuthorisationCallbackPayload Response { get; }
    }
}
