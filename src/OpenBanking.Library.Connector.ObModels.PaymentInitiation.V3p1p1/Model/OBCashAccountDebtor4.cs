/* 
 * Payment Initiation API
 *
 * OpenAPI for Payment Initiation API Specification
 *
 * The version of the OpenAPI document: v3.1.1
 * Contact: ServiceDesk@openbanking.org.uk
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model
{
    /// <summary>
    /// Provides the details to identify the debtor account.
    /// </summary>
    [DataContract]
    public partial class OBCashAccountDebtor4 :  IEquatable<OBCashAccountDebtor4>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBCashAccountDebtor4" /> class.
        /// </summary>
        [JsonConstructor]
        protected OBCashAccountDebtor4() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBCashAccountDebtor4" /> class.
        /// </summary>
        /// <param name="schemeName">Name of the identification scheme, in a coded form as published in an external list. (required).</param>
        /// <param name="identification">Identification assigned by an institution to identify an account. This identification is known by the account owner. (required).</param>
        /// <param name="name">Name of the account, as assigned by the account servicing institution. Usage: The account name is the name or names of the account owner(s) represented at an account level. The account name is not the product name or the nickname of the account..</param>
        /// <param name="secondaryIdentification">This is secondary identification of the account, as assigned by the account servicing institution.  This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination)..</param>
        public OBCashAccountDebtor4(string schemeName = default(string), string identification = default(string), string name = default(string), string secondaryIdentification = default(string))
        {
            // to ensure "schemeName" is required (not null)
            this.SchemeName = schemeName ?? throw new ArgumentNullException("schemeName is a required property for OBCashAccountDebtor4 and cannot be null");
            // to ensure "identification" is required (not null)
            this.Identification = identification ?? throw new ArgumentNullException("identification is a required property for OBCashAccountDebtor4 and cannot be null");
            this.Name = name;
            this.SecondaryIdentification = secondaryIdentification;
        }
        
        /// <summary>
        /// Name of the identification scheme, in a coded form as published in an external list.
        /// </summary>
        /// <value>Name of the identification scheme, in a coded form as published in an external list.</value>
        [DataMember(Name="SchemeName", EmitDefaultValue=false)]
        public string SchemeName { get; set; }

        /// <summary>
        /// Identification assigned by an institution to identify an account. This identification is known by the account owner.
        /// </summary>
        /// <value>Identification assigned by an institution to identify an account. This identification is known by the account owner.</value>
        [DataMember(Name="Identification", EmitDefaultValue=false)]
        public string Identification { get; set; }

        /// <summary>
        /// Name of the account, as assigned by the account servicing institution. Usage: The account name is the name or names of the account owner(s) represented at an account level. The account name is not the product name or the nickname of the account.
        /// </summary>
        /// <value>Name of the account, as assigned by the account servicing institution. Usage: The account name is the name or names of the account owner(s) represented at an account level. The account name is not the product name or the nickname of the account.</value>
        [DataMember(Name="Name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// This is secondary identification of the account, as assigned by the account servicing institution.  This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).
        /// </summary>
        /// <value>This is secondary identification of the account, as assigned by the account servicing institution.  This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).</value>
        [DataMember(Name="SecondaryIdentification", EmitDefaultValue=false)]
        public string SecondaryIdentification { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OBCashAccountDebtor4 {\n");
            sb.Append("  SchemeName: ").Append(SchemeName).Append("\n");
            sb.Append("  Identification: ").Append(Identification).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  SecondaryIdentification: ").Append(SecondaryIdentification).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as OBCashAccountDebtor4);
        }

        /// <summary>
        /// Returns true if OBCashAccountDebtor4 instances are equal
        /// </summary>
        /// <param name="input">Instance of OBCashAccountDebtor4 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBCashAccountDebtor4 input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.SchemeName == input.SchemeName ||
                    (this.SchemeName != null &&
                    this.SchemeName.Equals(input.SchemeName))
                ) && 
                (
                    this.Identification == input.Identification ||
                    (this.Identification != null &&
                    this.Identification.Equals(input.Identification))
                ) && 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.SecondaryIdentification == input.SecondaryIdentification ||
                    (this.SecondaryIdentification != null &&
                    this.SecondaryIdentification.Equals(input.SecondaryIdentification))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.SchemeName != null)
                    hashCode = hashCode * 59 + this.SchemeName.GetHashCode();
                if (this.Identification != null)
                    hashCode = hashCode * 59 + this.Identification.GetHashCode();
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.SecondaryIdentification != null)
                    hashCode = hashCode * 59 + this.SecondaryIdentification.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            // SchemeName (string) maxLength
            if(this.SchemeName != null && this.SchemeName.Length > 40)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SchemeName, length must be less than 40.", new [] { "SchemeName" });
            }

            // SchemeName (string) minLength
            if(this.SchemeName != null && this.SchemeName.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SchemeName, length must be greater than 1.", new [] { "SchemeName" });
            }

            // Identification (string) maxLength
            if(this.Identification != null && this.Identification.Length > 256)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Identification, length must be less than 256.", new [] { "Identification" });
            }

            // Identification (string) minLength
            if(this.Identification != null && this.Identification.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Identification, length must be greater than 1.", new [] { "Identification" });
            }

            // Name (string) maxLength
            if(this.Name != null && this.Name.Length > 70)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Name, length must be less than 70.", new [] { "Name" });
            }

            // Name (string) minLength
            if(this.Name != null && this.Name.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Name, length must be greater than 1.", new [] { "Name" });
            }

            // SecondaryIdentification (string) maxLength
            if(this.SecondaryIdentification != null && this.SecondaryIdentification.Length > 34)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SecondaryIdentification, length must be less than 34.", new [] { "SecondaryIdentification" });
            }

            // SecondaryIdentification (string) minLength
            if(this.SecondaryIdentification != null && this.SecondaryIdentification.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for SecondaryIdentification, length must be greater than 1.", new [] { "SecondaryIdentification" });
            }

            yield break;
        }
    }

}
