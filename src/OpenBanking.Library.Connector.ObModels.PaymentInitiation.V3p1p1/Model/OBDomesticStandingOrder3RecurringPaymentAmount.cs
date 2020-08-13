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
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model
{
    /// <summary>
    /// The amount of the recurring Standing Order
    /// </summary>
    [DataContract]
    public partial class OBDomesticStandingOrder3RecurringPaymentAmount :  IEquatable<OBDomesticStandingOrder3RecurringPaymentAmount>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBDomesticStandingOrder3RecurringPaymentAmount" /> class.
        /// </summary>
        [JsonConstructor]
        protected OBDomesticStandingOrder3RecurringPaymentAmount() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBDomesticStandingOrder3RecurringPaymentAmount" /> class.
        /// </summary>
        /// <param name="amount">A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217. (required).</param>
        /// <param name="currency">currency (required).</param>
        public OBDomesticStandingOrder3RecurringPaymentAmount(string amount = default(string), string currency = default(string))
        {
            // to ensure "amount" is required (not null)
            this.Amount = amount ?? throw new ArgumentNullException("amount is a required property for OBDomesticStandingOrder3RecurringPaymentAmount and cannot be null");
            // to ensure "currency" is required (not null)
            this.Currency = currency ?? throw new ArgumentNullException("currency is a required property for OBDomesticStandingOrder3RecurringPaymentAmount and cannot be null");
        }
        
        /// <summary>
        /// A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217.
        /// </summary>
        /// <value>A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217.</value>
        [DataMember(Name="Amount", EmitDefaultValue=false)]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or Sets Currency
        /// </summary>
        [DataMember(Name="Currency", EmitDefaultValue=false)]
        public string Currency { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OBDomesticStandingOrder3RecurringPaymentAmount {\n");
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
            return this.Equals(input as OBDomesticStandingOrder3RecurringPaymentAmount);
        }

        /// <summary>
        /// Returns true if OBDomesticStandingOrder3RecurringPaymentAmount instances are equal
        /// </summary>
        /// <param name="input">Instance of OBDomesticStandingOrder3RecurringPaymentAmount to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBDomesticStandingOrder3RecurringPaymentAmount input)
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
