﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class OpenIdConfigurationOverrides
    {
        [JsonProperty("registrationEndpointUrl")]
        public string RegistrationEndpointUrl { get; set; }
    }
}
