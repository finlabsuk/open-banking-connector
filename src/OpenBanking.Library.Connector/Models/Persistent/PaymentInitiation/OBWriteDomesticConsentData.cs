using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     OBWriteDomesticConsentData
    /// </summary>
    public class OBWriteDomesticConsentData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticConsentData" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticConsentData()
        {
        }

        /// <summary>
        ///     Gets or Sets Initiation
        /// </summary>
        [JsonProperty("initiation")]
        public OBWriteDomesticDataInitiation Initiation { get; set; }

        /// <summary>
        ///     Gets or Sets Authorisation
        /// </summary>
        [JsonProperty("authorisation")]
        public OBWriteDomesticConsentDataAuthorisation Authorisation { get; set; }

        /// <summary>
        ///     Gets or Sets SCASupportData
        /// </summary>
        [JsonProperty("scaSupportData")]
        public OBWriteDomesticConsentDataSCASupportData SCASupportData { get; set; }
    }
}