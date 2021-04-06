// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    [Owned]
    public class OpenIdConfiguration
    {
        [JsonProperty("issuer")]
        public string Issuer { get; set; } = null!;

        [JsonProperty("response_types_supported")]
        public IList<string> ResponseTypesSupported { get; set; } = null!;

        [JsonProperty("scopes_supported")]
        public IList<string> ScopesSupported { get; set; } = null!;

        [JsonProperty("response_modes_supported")]
        public IList<string> ResponseModesSupported { get; set; } = null!;

        [JsonProperty("token_endpoint")]
        public string TokenEndpoint { get; set; } = null!;

        [JsonProperty("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; } = null!;

        [JsonProperty("registration_endpoint")]
        public string RegistrationEndpoint { get; set; } = null!;
    }
}
