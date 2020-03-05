using FinnovationLabs.OpenBanking.Library.Connector.Models.Converters;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation
{
    /// <summary>
    ///     OBWriteDomesticConsentData
    /// </summary>
    [OpenBankingEquivalent(typeof(OBWriteDataDomesticConsent2), EquivalentTypeMapper = typeof(OBWriteDomesticConsentDataConverter))]
    [OpenBankingEquivalent(typeof(OBWriteDataDomestic2))]
    [OpenBankingEquivalent(typeof(OBWriteDomesticConsent3Data))]
    [PersistenceEquivalent(typeof(Persistent.PaymentInitiation.OBWriteDomesticConsentData))]
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
        [JsonProperty("scaSupportData", Required = Required.Default)]
        public OBWriteDomesticConsentDataSCASupportData SCASupportData { get; set; }
    }
}