// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class OBBranchAndFinancialInstitutionIdentification6
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBBranchAndFinancialInstitutionIdentification6 class.
        /// </summary>
        public OBBranchAndFinancialInstitutionIdentification6()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBBranchAndFinancialInstitutionIdentification6 class.
        /// </summary>
        /// <param name="schemeName">^ 0..1) | `SchemeName` |Name of the
        /// identification scheme, in a coded form as published in an external
        /// list. |OBExternalFinancialInstitutionIdentification4Code</param>
        /// <param name="identification">^ 0..1) | `Identification` |Unique and
        /// unambiguous identification of a financial institution or a branch
        /// of a financial institution.  | Max35Text</param>
        /// <param name="name">^ 0..1) | `Name` | Name by which an agent is
        /// known and which is usually used to identify that agent. |
        /// Max140Text</param>
        /// <param name="postalAddress">^ 0..1) | `PostalAddress` |Information
        /// that locates and identifies a specific address, as defined by
        /// postal services.| OBPostalAddress6</param>
        public OBBranchAndFinancialInstitutionIdentification6(string schemeName = default(string), string identification = default(string), string name = default(string), OBBranchAndFinancialInstitutionIdentification6PostalAddress postalAddress = default(OBBranchAndFinancialInstitutionIdentification6PostalAddress))
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
        /// Gets or sets ^ 0..1) | `SchemeName` |Name of the identification
        /// scheme, in a coded form as published in an external list.
        /// |OBExternalFinancialInstitutionIdentification4Code
        /// </summary>
        [JsonProperty(PropertyName = "SchemeName")]
        public string SchemeName { get; set; }

        /// <summary>
        /// Gets or sets ^ 0..1) | `Identification` |Unique and unambiguous
        /// identification of a financial institution or a branch of a
        /// financial institution.  | Max35Text
        /// </summary>
        [JsonProperty(PropertyName = "Identification")]
        public string Identification { get; set; }

        /// <summary>
        /// Gets or sets ^ 0..1) | `Name` | Name by which an agent is known and
        /// which is usually used to identify that agent. | Max140Text
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets ^ 0..1) | `PostalAddress` |Information that locates
        /// and identifies a specific address, as defined by postal services.|
        /// OBPostalAddress6
        /// </summary>
        [JsonProperty(PropertyName = "PostalAddress")]
        public OBBranchAndFinancialInstitutionIdentification6PostalAddress PostalAddress { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (PostalAddress != null)
            {
                PostalAddress.Validate();
            }
        }
    }
}
