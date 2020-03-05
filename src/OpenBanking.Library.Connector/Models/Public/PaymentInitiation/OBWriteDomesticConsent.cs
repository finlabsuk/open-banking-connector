using FinnovationLabs.OpenBanking.Library.Connector.Models.Converters;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using Newtonsoft.Json;
using OBWriteDomestic2 = FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomestic2;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation
{
    /// <summary>
    ///     OBWriteDomesticConsent
    /// </summary>
    [OpenBankingEquivalent(typeof(OBWriteDomesticConsent2), EquivalentTypeMapper = typeof(OBWriteDomesticConsentConverter))]
    [OpenBankingEquivalent(typeof(OBWriteDomestic2), EquivalentTypeMapper = typeof(OBWriteDomesticConsentOBWriteDomestic2Converter))]
    [OpenBankingEquivalent(typeof(OBWriteDomesticConsent3))]
    [PersistenceEquivalent(typeof(DomesticConsent), EquivalentTypeMapper = typeof(Converters.Persistent.OBWriteDomesticConsentConverter))]
    [PersistenceEquivalent(typeof(Persistent.PaymentInitiation.OBWriteDomesticConsent))]
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
        [JsonProperty("apiProfileId", Required = Required.Always)]
        public string ApiProfileId { get; set; }

        /// <summary>
        ///     Gets or Sets Data
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public OBWriteDomesticConsentData Data { get; set; }

        /// <summary>
        ///     Gets or Sets Risk
        /// </summary>
        [JsonProperty("risk", Required = Required.Default)]
        public OBRisk Risk { get; set; }

 
    }
}