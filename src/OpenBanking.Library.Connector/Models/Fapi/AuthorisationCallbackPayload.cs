﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi
{
    public class AuthorisationCallbackPayload
    {
        [JsonProperty("id_token")]
        public string Id_Token { get; set; } = null!;

        [JsonProperty("code")]
        public string Code { get; set; } = null!;

        [JsonProperty("state")]
        public string State { get; set; } = null!;

        [JsonProperty("nonce", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Nonce { get; set; }
    }
}
