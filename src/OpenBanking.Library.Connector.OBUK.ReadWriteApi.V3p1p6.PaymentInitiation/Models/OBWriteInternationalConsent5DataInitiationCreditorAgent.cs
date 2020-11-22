// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p6.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Financial institution servicing an account for the creditor.
    /// </summary>
    public partial class OBWriteInternationalConsent5DataInitiationCreditorAgent
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalConsent5DataInitiationCreditorAgent class.
        /// </summary>
        public OBWriteInternationalConsent5DataInitiationCreditorAgent()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalConsent5DataInitiationCreditorAgent class.
        /// </summary>
        public OBWriteInternationalConsent5DataInitiationCreditorAgent(string schemeName = default(string), string identification = default(string), string name = default(string), OBPostalAddress6 postalAddress = default(OBPostalAddress6))
        {
            SchemeName = schemeName;
            Identification = identification;
            Name = name;
            PostalAddress = postalAddress;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SchemeName")]
        public string SchemeName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Identification")]
        public string Identification { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PostalAddress")]
        public OBPostalAddress6 PostalAddress { get; set; }

    }
}
