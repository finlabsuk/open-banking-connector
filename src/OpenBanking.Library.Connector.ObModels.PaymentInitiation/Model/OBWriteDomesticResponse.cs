// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    [SourceApiEquivalent(typeof(OBWriteDomesticResponse2))]
    public class OBWriteDomesticResponse
    {
        [JsonProperty("data")]
        public OBWriteDataDomesticResponse Data { get; set; }

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Links Links { get; set; }

        [JsonProperty("meta", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Meta Meta { get; set; }
    }
}
