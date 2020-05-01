// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{

    public class BankClientRegistrationData
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string? ClientSecret { get; set; }

        [JsonProperty("client_id_issued_at")]
        public DateTimeOffset ClientIdIssuedAt { get; set; }

        [JsonProperty("client_secret_expires_at")]
        public DateTimeOffset ClientSecretExpiresAt { get; set; }
    }
}
