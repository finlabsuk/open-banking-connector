using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation
{
    /// <summary>
    ///     OBWriteDomesticConsent
    /// </summary>
    public class OBWriteDomesticConsent
    {
        

        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticConsent" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticConsent()
        {
        }


        /// <summary>
        ///  OBClientProfile ID to use
        /// </summary>
        [JsonProperty("openBankingClientProfileId")]
        public string OpenBankingClientProfileId { get; set; }

        /// <summary>
        ///     Gets or Sets Data
        /// </summary>
        [JsonProperty("data")]
        public OBWriteDomesticConsentData Data { get; set; }

        /// <summary>
        ///     Gets or Sets Risk
        /// </summary>
        [JsonProperty("risk")]
        public OBRisk Risk { get; set; }
    }
}