// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p4.PaymentInitiation.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Identification assigned by an institution to identify an account. This
    /// identification is known by the account owner.
    /// </summary>
    public partial class OBWriteDomesticStandingOrderConsentResponse5DataInitiationCreditorAccount
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticStandingOrderConsentResponse5DataInitiationCreditorAccount
        /// class.
        /// </summary>
        public OBWriteDomesticStandingOrderConsentResponse5DataInitiationCreditorAccount()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticStandingOrderConsentResponse5DataInitiationCreditorAccount
        /// class.
        /// </summary>
        /// <param name="name">The account name is the name or names of the
        /// account owner(s) represented at an account level.
        /// Note, the account name is not the product name or the nickname of
        /// the account.
        /// OB: ASPSPs may carry out name validation for Confirmation of Payee,
        /// but it is not mandatory.</param>
        public OBWriteDomesticStandingOrderConsentResponse5DataInitiationCreditorAccount(string schemeName, string identification, string name, string secondaryIdentification = default(string))
        {
            SchemeName = schemeName;
            Identification = identification;
            Name = name;
            SecondaryIdentification = secondaryIdentification;
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
        /// Gets or sets the account name is the name or names of the account
        /// owner(s) represented at an account level.
        /// Note, the account name is not the product name or the nickname of
        /// the account.
        /// OB: ASPSPs may carry out name validation for Confirmation of Payee,
        /// but it is not mandatory.
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SecondaryIdentification")]
        public string SecondaryIdentification { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (SchemeName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SchemeName");
            }
            if (Identification == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Identification");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
        }
    }
}
