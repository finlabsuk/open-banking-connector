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
    /// The Risk section is sent by the initiating party to the ASPSP. It is used to specify additional details for risk scoring for Payments.
    /// </summary>
    [DataContract]
    public partial class OBRisk1 :  IEquatable<OBRisk1>, IValidatableObject
    {
        /// <summary>
        /// Gets or Sets PaymentContextCode
        /// </summary>
        [DataMember(Name="PaymentContextCode", EmitDefaultValue=false)]
        public OBExternalPaymentContext1Code? PaymentContextCode { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBRisk1" /> class.
        /// </summary>
        /// <param name="paymentContextCode">paymentContextCode.</param>
        /// <param name="merchantCategoryCode">Category code conform to ISO 18245, related to the type of services or goods the merchant provides for the transaction..</param>
        /// <param name="merchantCustomerIdentification">The unique customer identifier of the PSU with the merchant..</param>
        /// <param name="deliveryAddress">deliveryAddress.</param>
        public OBRisk1(OBExternalPaymentContext1Code? paymentContextCode = default(OBExternalPaymentContext1Code?), string merchantCategoryCode = default(string), string merchantCustomerIdentification = default(string), OBRisk1DeliveryAddress deliveryAddress = default(OBRisk1DeliveryAddress))
        {
            this.PaymentContextCode = paymentContextCode;
            this.MerchantCategoryCode = merchantCategoryCode;
            this.MerchantCustomerIdentification = merchantCustomerIdentification;
            this.DeliveryAddress = deliveryAddress;
        }
        
        /// <summary>
        /// Category code conform to ISO 18245, related to the type of services or goods the merchant provides for the transaction.
        /// </summary>
        /// <value>Category code conform to ISO 18245, related to the type of services or goods the merchant provides for the transaction.</value>
        [DataMember(Name="MerchantCategoryCode", EmitDefaultValue=false)]
        public string MerchantCategoryCode { get; set; }

        /// <summary>
        /// The unique customer identifier of the PSU with the merchant.
        /// </summary>
        /// <value>The unique customer identifier of the PSU with the merchant.</value>
        [DataMember(Name="MerchantCustomerIdentification", EmitDefaultValue=false)]
        public string MerchantCustomerIdentification { get; set; }

        /// <summary>
        /// Gets or Sets DeliveryAddress
        /// </summary>
        [DataMember(Name="DeliveryAddress", EmitDefaultValue=false)]
        public OBRisk1DeliveryAddress DeliveryAddress { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OBRisk1 {\n");
            sb.Append("  PaymentContextCode: ").Append(PaymentContextCode).Append("\n");
            sb.Append("  MerchantCategoryCode: ").Append(MerchantCategoryCode).Append("\n");
            sb.Append("  MerchantCustomerIdentification: ").Append(MerchantCustomerIdentification).Append("\n");
            sb.Append("  DeliveryAddress: ").Append(DeliveryAddress).Append("\n");
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
            return this.Equals(input as OBRisk1);
        }

        /// <summary>
        /// Returns true if OBRisk1 instances are equal
        /// </summary>
        /// <param name="input">Instance of OBRisk1 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBRisk1 input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.PaymentContextCode == input.PaymentContextCode ||
                    this.PaymentContextCode.Equals(input.PaymentContextCode)
                ) && 
                (
                    this.MerchantCategoryCode == input.MerchantCategoryCode ||
                    (this.MerchantCategoryCode != null &&
                    this.MerchantCategoryCode.Equals(input.MerchantCategoryCode))
                ) && 
                (
                    this.MerchantCustomerIdentification == input.MerchantCustomerIdentification ||
                    (this.MerchantCustomerIdentification != null &&
                    this.MerchantCustomerIdentification.Equals(input.MerchantCustomerIdentification))
                ) && 
                (
                    this.DeliveryAddress == input.DeliveryAddress ||
                    (this.DeliveryAddress != null &&
                    this.DeliveryAddress.Equals(input.DeliveryAddress))
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
                hashCode = hashCode * 59 + this.PaymentContextCode.GetHashCode();
                if (this.MerchantCategoryCode != null)
                    hashCode = hashCode * 59 + this.MerchantCategoryCode.GetHashCode();
                if (this.MerchantCustomerIdentification != null)
                    hashCode = hashCode * 59 + this.MerchantCustomerIdentification.GetHashCode();
                if (this.DeliveryAddress != null)
                    hashCode = hashCode * 59 + this.DeliveryAddress.GetHashCode();
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
            // MerchantCategoryCode (string) maxLength
            if(this.MerchantCategoryCode != null && this.MerchantCategoryCode.Length > 4)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for MerchantCategoryCode, length must be less than 4.", new [] { "MerchantCategoryCode" });
            }

            // MerchantCategoryCode (string) minLength
            if(this.MerchantCategoryCode != null && this.MerchantCategoryCode.Length < 3)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for MerchantCategoryCode, length must be greater than 3.", new [] { "MerchantCategoryCode" });
            }

            // MerchantCustomerIdentification (string) maxLength
            if(this.MerchantCustomerIdentification != null && this.MerchantCustomerIdentification.Length > 70)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for MerchantCustomerIdentification, length must be less than 70.", new [] { "MerchantCustomerIdentification" });
            }

            // MerchantCustomerIdentification (string) minLength
            if(this.MerchantCustomerIdentification != null && this.MerchantCustomerIdentification.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for MerchantCustomerIdentification, length must be greater than 1.", new [] { "MerchantCustomerIdentification" });
            }

            yield break;
        }
    }

}
