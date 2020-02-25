using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation
{
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
