using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation
{
    public class OBCharge
    {
        [JsonProperty("chargeBearer")]
        public OBChargeBearerType1Code ChargeBearer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("amount")]
        public OBChargeAmount Amount { get; set; }
    }
}
