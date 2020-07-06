using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    public class OBChargeAmount
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }
        
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
