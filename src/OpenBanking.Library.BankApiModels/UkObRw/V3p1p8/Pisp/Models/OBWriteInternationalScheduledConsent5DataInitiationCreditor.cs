// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Party to which an amount of money is due.
    /// </summary>
    public partial class OBWriteInternationalScheduledConsent5DataInitiationCreditor
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsent5DataInitiationCreditor class.
        /// </summary>
        public OBWriteInternationalScheduledConsent5DataInitiationCreditor()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsent5DataInitiationCreditor class.
        /// </summary>
        /// <param name="name">Name by which a party is known and which is
        /// usually used to identify that party.</param>
        public OBWriteInternationalScheduledConsent5DataInitiationCreditor(string name = default(string), OBPostalAddress6 postalAddress = default(OBPostalAddress6))
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

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name != null)
            {
                if (Name.Length > 140)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Name", 140);
                }
                if (Name.Length < 1)
                {
                    throw new ValidationException(ValidationRules.MinLength, "Name", 1);
                }
            }
            if (PostalAddress != null)
            {
                PostalAddress.Validate();
            }
        }
    }
}
