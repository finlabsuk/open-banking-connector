// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class RegistrationResponseJsonOptions
    {
        [JsonProperty("grant_types")]
        public IList<string> GrantTypes { get; set; }

        public DateTimeOffsetUnixConverterOptions DateTimeOffsetUnixConverterOptions { get; set; }

        public DelimitedStringConverterOptions DelimitedStringConverterOptions { get; set; }
    }
}
