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
    /// OBError1
    /// </summary>
    [DataContract]
    public partial class OBError1 :  IEquatable<OBError1>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBError1" /> class.
        /// </summary>
        [JsonConstructor]
        protected OBError1() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBError1" /> class.
        /// </summary>
        /// <param name="errorCode">Low level textual error code, e.g., UK.OBIE.Field.Missing (required).</param>
        /// <param name="message">A description of the error that occurred. e.g., &#39;A mandatory field isn&#39;t supplied&#39; or &#39;RequestedExecutionDateTime must be in future&#39; OBIE doesn&#39;t standardise this field (required).</param>
        /// <param name="path">Recommended but optional reference to the JSON Path of the field with error, e.g., Data.Initiation.InstructedAmount.Currency.</param>
        /// <param name="url">URL to help remediate the problem, or provide more information, or to API Reference, or help etc.</param>
        public OBError1(string errorCode = default(string), string message = default(string), string path = default(string), string url = default(string))
        {
            // to ensure "errorCode" is required (not null)
            this.ErrorCode = errorCode ?? throw new ArgumentNullException("errorCode is a required property for OBError1 and cannot be null");
            // to ensure "message" is required (not null)
            this.Message = message ?? throw new ArgumentNullException("message is a required property for OBError1 and cannot be null");
            this.Path = path;
            this.Url = url;
        }
        
        /// <summary>
        /// Low level textual error code, e.g., UK.OBIE.Field.Missing
        /// </summary>
        /// <value>Low level textual error code, e.g., UK.OBIE.Field.Missing</value>
        [DataMember(Name="ErrorCode", EmitDefaultValue=false)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// A description of the error that occurred. e.g., &#39;A mandatory field isn&#39;t supplied&#39; or &#39;RequestedExecutionDateTime must be in future&#39; OBIE doesn&#39;t standardise this field
        /// </summary>
        /// <value>A description of the error that occurred. e.g., &#39;A mandatory field isn&#39;t supplied&#39; or &#39;RequestedExecutionDateTime must be in future&#39; OBIE doesn&#39;t standardise this field</value>
        [DataMember(Name="Message", EmitDefaultValue=false)]
        public string Message { get; set; }

        /// <summary>
        /// Recommended but optional reference to the JSON Path of the field with error, e.g., Data.Initiation.InstructedAmount.Currency
        /// </summary>
        /// <value>Recommended but optional reference to the JSON Path of the field with error, e.g., Data.Initiation.InstructedAmount.Currency</value>
        [DataMember(Name="Path", EmitDefaultValue=false)]
        public string Path { get; set; }

        /// <summary>
        /// URL to help remediate the problem, or provide more information, or to API Reference, or help etc
        /// </summary>
        /// <value>URL to help remediate the problem, or provide more information, or to API Reference, or help etc</value>
        [DataMember(Name="Url", EmitDefaultValue=false)]
        public string Url { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OBError1 {\n");
            sb.Append("  ErrorCode: ").Append(ErrorCode).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  Path: ").Append(Path).Append("\n");
            sb.Append("  Url: ").Append(Url).Append("\n");
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
            return this.Equals(input as OBError1);
        }

        /// <summary>
        /// Returns true if OBError1 instances are equal
        /// </summary>
        /// <param name="input">Instance of OBError1 to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBError1 input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.ErrorCode == input.ErrorCode ||
                    (this.ErrorCode != null &&
                    this.ErrorCode.Equals(input.ErrorCode))
                ) && 
                (
                    this.Message == input.Message ||
                    (this.Message != null &&
                    this.Message.Equals(input.Message))
                ) && 
                (
                    this.Path == input.Path ||
                    (this.Path != null &&
                    this.Path.Equals(input.Path))
                ) && 
                (
                    this.Url == input.Url ||
                    (this.Url != null &&
                    this.Url.Equals(input.Url))
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
                if (this.ErrorCode != null)
                    hashCode = hashCode * 59 + this.ErrorCode.GetHashCode();
                if (this.Message != null)
                    hashCode = hashCode * 59 + this.Message.GetHashCode();
                if (this.Path != null)
                    hashCode = hashCode * 59 + this.Path.GetHashCode();
                if (this.Url != null)
                    hashCode = hashCode * 59 + this.Url.GetHashCode();
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
            // ErrorCode (string) maxLength
            if(this.ErrorCode != null && this.ErrorCode.Length > 128)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for ErrorCode, length must be less than 128.", new [] { "ErrorCode" });
            }

            // ErrorCode (string) minLength
            if(this.ErrorCode != null && this.ErrorCode.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for ErrorCode, length must be greater than 1.", new [] { "ErrorCode" });
            }

            // Message (string) maxLength
            if(this.Message != null && this.Message.Length > 500)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Message, length must be less than 500.", new [] { "Message" });
            }

            // Message (string) minLength
            if(this.Message != null && this.Message.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Message, length must be greater than 1.", new [] { "Message" });
            }

            // Path (string) maxLength
            if(this.Path != null && this.Path.Length > 500)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Path, length must be less than 500.", new [] { "Path" });
            }

            // Path (string) minLength
            if(this.Path != null && this.Path.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Path, length must be greater than 1.", new [] { "Path" });
            }

            yield break;
        }
    }

}
