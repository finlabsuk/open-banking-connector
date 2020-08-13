// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.WebHost.Entities
{
    public class MessagesResponse
    {
        [JsonProperty("info", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<string>? InformationMessages { get; set; }

        [JsonProperty("warnings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<string>? WarningMessages { get; set; }

        [JsonProperty("errors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<string>? ErrorMessages { get; set; }
    }
}
