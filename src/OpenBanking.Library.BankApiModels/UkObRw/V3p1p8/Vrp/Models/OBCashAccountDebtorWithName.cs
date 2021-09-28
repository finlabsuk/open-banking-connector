// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// ^ Only incuded in the response if `Data. ReadRefundAccount` is set to
    /// `Yes` in the consent.
    /// </summary>
    public partial class OBCashAccountDebtorWithName
    {
        /// <summary>
        /// Initializes a new instance of the OBCashAccountDebtorWithName
        /// class.
        /// </summary>
        public OBCashAccountDebtorWithName()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBCashAccountDebtorWithName
        /// class.
        /// </summary>
        /// <param name="schemeName">^ Name of the identification scheme, in a
        /// coded form as published in an external list. | Namespaced
        /// Enumeration OBExternalAccountIdentification4Code</param>
        /// <param name="identification">^ Identification assigned by an
        /// institution to identify an account. This identification is known by
        /// the account owner. | Max256Text</param>
        /// <param name="name">^ Name of the account, as assigned by the
        /// account servicing institution.  Usage The account name is the name
        /// or names of the account owner(s) represented at an account level.
        /// The account name is not the product name or the nickname of the
        /// account.</param>
        /// <param name="secondaryIdentification">^ This is secondary
        /// identification of the account, as assigned by the account servicing
        /// institution.  This can be used by building societies to
        /// additionally identify accounts with a roll number (in addition to a
        /// sort code and account number combination) | Max34Text</param>
        public OBCashAccountDebtorWithName(string schemeName, string identification, string name, string secondaryIdentification = default(string))
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
        /// Gets or sets ^ Name of the identification scheme, in a coded form
        /// as published in an external list. | Namespaced Enumeration
        /// OBExternalAccountIdentification4Code
        /// </summary>
        [JsonProperty(PropertyName = "SchemeName")]
        public string SchemeName { get; set; }

        /// <summary>
        /// Gets or sets ^ Identification assigned by an institution to
        /// identify an account. This identification is known by the account
        /// owner. | Max256Text
        /// </summary>
        [JsonProperty(PropertyName = "Identification")]
        public string Identification { get; set; }

        /// <summary>
        /// Gets or sets ^ Name of the account, as assigned by the account
        /// servicing institution.  Usage The account name is the name or names
        /// of the account owner(s) represented at an account level. The
        /// account name is not the product name or the nickname of the
        /// account.
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets ^ This is secondary identification of the account, as
        /// assigned by the account servicing institution.  This can be used by
        /// building societies to additionally identify accounts with a roll
        /// number (in addition to a sort code and account number combination)
        /// | Max34Text
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
