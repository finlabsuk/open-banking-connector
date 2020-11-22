// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p2.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Party to which an amount of money is due.
    /// </summary>
    public partial class OBWriteInternationalScheduledConsentResponse3DataInitiationCreditor
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsentResponse3DataInitiationCreditor
        /// class.
        /// </summary>
        public OBWriteInternationalScheduledConsentResponse3DataInitiationCreditor()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsentResponse3DataInitiationCreditor
        /// class.
        /// </summary>
        /// <param name="name">Name by which a party is known and which is
        /// usually used to identify that party.</param>
        public OBWriteInternationalScheduledConsentResponse3DataInitiationCreditor(string name = default(string), OBPostalAddress6 postalAddress = default(OBPostalAddress6))
        {
            Name = name;
            PostalAddress = postalAddress;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets name by which a party is known and which is usually
        /// used to identify that party.
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PostalAddress")]
        public OBPostalAddress6 PostalAddress { get; set; }

    }
}
