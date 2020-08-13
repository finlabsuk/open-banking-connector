/* 
 * Payment Initiation API
 *
 * OpenAPI for Payment Initiation API Specification
 *
 * The version of the OpenAPI document: v3.1.4
 * Contact: ServiceDesk@openbanking.org.uk
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model
{
    /// <summary>
    /// The amount of the final Standing Order
    /// </summary>
    [DataContract]
    public partial class OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount :  IEquatable<OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount" /> class.
        /// </summary>
        [JsonConstructor]
        protected OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount" /> class.
        /// </summary>
        /// <param name="amount">A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217. (required).</param>
        /// <param name="currency">A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 \&quot;Codes for the representation of currencies and funds\&quot;. (required).</param>
        public OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount(string amount = default(string), string currency = default(string))
        {
            // to ensure "amount" is required (not null)
            this.Amount = amount ?? throw new ArgumentNullException("amount is a required property for OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount and cannot be null");
            // to ensure "currency" is required (not null)
            this.Currency = currency ?? throw new ArgumentNullException("currency is a required property for OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount and cannot be null");
        }
        
        /// <summary>
        /// A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217.
        /// </summary>
        /// <value>A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217.</value>
        [DataMember(Name="Amount", EmitDefaultValue=false)]
        public string Amount { get; set; }

        /// <summary>
        /// A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 \&quot;Codes for the representation of currencies and funds\&quot;.
        /// </summary>
        /// <value>A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 \&quot;Codes for the representation of currencies and funds\&quot;.</value>
        [DataMember(Name="Currency", EmitDefaultValue=false)]
        public string Currency { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount {\n");
            sb.Append("  Amount: ").Append(Amount).Append("\n");
            sb.Append("  Currency: ").Append(Currency).Append("\n");
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
            return this.Equals(input as OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount);
        }

        /// <summary>
        /// Returns true if OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount instances are equal
        /// </summary>
        /// <param name="input">Instance of OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Amount == input.Amount ||
                    (this.Amount != null &&
                    this.Amount.Equals(input.Amount))
                ) && 
                (
                    this.Currency == input.Currency ||
                    (this.Currency != null &&
                    this.Currency.Equals(input.Currency))
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
                if (this.Amount != null)
                    hashCode = hashCode * 59 + this.Amount.GetHashCode();
                if (this.Currency != null)
                    hashCode = hashCode * 59 + this.Currency.GetHashCode();
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
            // Amount (string) pattern
            Regex regexAmount = new Regex(@"^\\d{1,13}\\.\\d{1,5}$", RegexOptions.CultureInvariant);
            if (false == regexAmount.Match(this.Amount).Success)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Amount, must match a pattern of " + regexAmount, new [] { "Amount" });
            }

            // Currency (string) pattern
            Regex regexCurrency = new Regex(@"^[A-Z]{3,3}$", RegexOptions.CultureInvariant);
            if (false == regexCurrency.Match(this.Currency).Success)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Currency, must match a pattern of " + regexCurrency, new [] { "Currency" });
            }

            yield break;
        }
    }

}
