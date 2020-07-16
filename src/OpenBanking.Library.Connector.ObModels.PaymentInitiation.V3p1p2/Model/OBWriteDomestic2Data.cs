/* 
 * Payment Initiation API
 *
 * OpenAPI for Payment Initiation API Specification
 *
 * The version of the OpenAPI document: v3.1.2
 * Contact: ServiceDesk@openbanking.org.uk
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model
{
    /// <summary>
    /// OBWriteDomestic2Data
    /// </summary>
    [DataContract]
    public partial class OBWriteDomestic2Data :  IEquatable<OBWriteDomestic2Data>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteDomestic2Data" /> class.
        /// </summary>
        [JsonConstructor]
        protected OBWriteDomestic2Data() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteDomestic2Data" /> class.
        /// </summary>
        /// <param name="consentId">OB: Unique identification as assigned by the ASPSP to uniquely identify the consent resource. (required).</param>
        /// <param name="initiation">initiation (required).</param>
        public OBWriteDomestic2Data(string consentId = default(string), OBWriteDomestic2DataInitiation initiation = default(OBWriteDomestic2DataInitiation))
        {
            // to ensure "consentId" is required (not null)
            this.ConsentId = consentId ?? throw new ArgumentNullException("consentId is a required property for OBWriteDomestic2Data and cannot be null");
            // to ensure "initiation" is required (not null)
            this.Initiation = initiation ?? throw new ArgumentNullException("initiation is a required property for OBWriteDomestic2Data and cannot be null");
        }
        
        /// <summary>
        /// OB: Unique identification as assigned by the ASPSP to uniquely identify the consent resource.
        /// </summary>
        /// <value>OB: Unique identification as assigned by the ASPSP to uniquely identify the consent resource.</value>
        [DataMember(Name="ConsentId", EmitDefaultValue=false)]
        public string ConsentId { get; set; }

        /// <summary>
        /// Gets or Sets Initiation
        /// </summary>
        [DataMember(Name="Initiation", EmitDefaultValue=false)]
        public OBWriteDomestic2DataInitiation Initiation { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OBWriteDomestic2Data {\n");
            sb.Append("  ConsentId: ").Append(ConsentId).Append("\n");
            sb.Append("  Initiation: ").Append(Initiation).Append("\n");
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
            return this.Equals(input as OBWriteDomestic2Data);
        }

        /// <summary>
        /// Returns true if OBWriteDomestic2Data instances are equal
        /// </summary>
        /// <param name="input">Instance of OBWriteDomestic2Data to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBWriteDomestic2Data input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.ConsentId == input.ConsentId ||
                    (this.ConsentId != null &&
                    this.ConsentId.Equals(input.ConsentId))
                ) && 
                (
                    this.Initiation == input.Initiation ||
                    (this.Initiation != null &&
                    this.Initiation.Equals(input.Initiation))
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
                if (this.ConsentId != null)
                    hashCode = hashCode * 59 + this.ConsentId.GetHashCode();
                if (this.Initiation != null)
                    hashCode = hashCode * 59 + this.Initiation.GetHashCode();
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
            // ConsentId (string) maxLength
            if(this.ConsentId != null && this.ConsentId.Length > 128)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for ConsentId, length must be less than 128.", new [] { "ConsentId" });
            }

            // ConsentId (string) minLength
            if(this.ConsentId != null && this.ConsentId.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for ConsentId, length must be greater than 1.", new [] { "ConsentId" });
            }

            yield break;
        }
    }

}
