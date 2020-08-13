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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model
{
    /// <summary>
    /// The Initiation payload is sent by the initiating party to the ASPSP. It is used to request movement of funds using a payment file.
    /// </summary>
    [DataContract]
    public partial class OBWriteFile2DataInitiation :  IEquatable<OBWriteFile2DataInitiation>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteFile2DataInitiation" /> class.
        /// </summary>
        [JsonConstructor]
        protected OBWriteFile2DataInitiation() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteFile2DataInitiation" /> class.
        /// </summary>
        /// <param name="fileType">Specifies the payment file type. (required).</param>
        /// <param name="fileHash">A base64 encoding of a SHA256 hash of the file to be uploaded. (required).</param>
        /// <param name="fileReference">Reference for the file..</param>
        /// <param name="numberOfTransactions">Number of individual transactions contained in the payment information group..</param>
        /// <param name="controlSum">Total of all individual amounts included in the group, irrespective of currencies..</param>
        /// <param name="requestedExecutionDateTime">Date at which the initiating party requests the clearing agent to process the payment.  Usage: This is the date on which the debtor&#39;s account is to be debited.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00.</param>
        /// <param name="localInstrument">User community specific instrument. Usage: This element is used to specify a local instrument, local clearing option and/or further qualify the service or service level..</param>
        /// <param name="debtorAccount">debtorAccount.</param>
        /// <param name="remittanceInformation">remittanceInformation.</param>
        /// <param name="supplementaryData">Additional information that can not be captured in the structured fields and/or any other specific block..</param>
        public OBWriteFile2DataInitiation(string fileType = default(string), string fileHash = default(string), string fileReference = default(string), string numberOfTransactions = default(string), decimal controlSum = default(decimal), DateTimeOffset requestedExecutionDateTime = default(DateTimeOffset), string localInstrument = default(string), OBWriteDomestic2DataInitiationDebtorAccount debtorAccount = default(OBWriteDomestic2DataInitiationDebtorAccount), OBWriteDomestic2DataInitiationRemittanceInformation remittanceInformation = default(OBWriteDomestic2DataInitiationRemittanceInformation), Dictionary<string, Object> supplementaryData = default(Dictionary<string, Object>))
        {
            // to ensure "fileType" is required (not null)
            this.FileType = fileType ?? throw new ArgumentNullException("fileType is a required property for OBWriteFile2DataInitiation and cannot be null");
            // to ensure "fileHash" is required (not null)
            this.FileHash = fileHash ?? throw new ArgumentNullException("fileHash is a required property for OBWriteFile2DataInitiation and cannot be null");
            this.FileReference = fileReference;
            this.NumberOfTransactions = numberOfTransactions;
            this.ControlSum = controlSum;
            this.RequestedExecutionDateTime = requestedExecutionDateTime;
            this.LocalInstrument = localInstrument;
            this.DebtorAccount = debtorAccount;
            this.RemittanceInformation = remittanceInformation;
            this.SupplementaryData = supplementaryData;
        }
        
        /// <summary>
        /// Specifies the payment file type.
        /// </summary>
        /// <value>Specifies the payment file type.</value>
        [DataMember(Name="FileType", EmitDefaultValue=false)]
        public string FileType { get; set; }

        /// <summary>
        /// A base64 encoding of a SHA256 hash of the file to be uploaded.
        /// </summary>
        /// <value>A base64 encoding of a SHA256 hash of the file to be uploaded.</value>
        [DataMember(Name="FileHash", EmitDefaultValue=false)]
        public string FileHash { get; set; }

        /// <summary>
        /// Reference for the file.
        /// </summary>
        /// <value>Reference for the file.</value>
        [DataMember(Name="FileReference", EmitDefaultValue=false)]
        public string FileReference { get; set; }

        /// <summary>
        /// Number of individual transactions contained in the payment information group.
        /// </summary>
        /// <value>Number of individual transactions contained in the payment information group.</value>
        [DataMember(Name="NumberOfTransactions", EmitDefaultValue=false)]
        public string NumberOfTransactions { get; set; }

        /// <summary>
        /// Total of all individual amounts included in the group, irrespective of currencies.
        /// </summary>
        /// <value>Total of all individual amounts included in the group, irrespective of currencies.</value>
        [DataMember(Name="ControlSum", EmitDefaultValue=false)]
        public decimal ControlSum { get; set; }

        /// <summary>
        /// Date at which the initiating party requests the clearing agent to process the payment.  Usage: This is the date on which the debtor&#39;s account is to be debited.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>Date at which the initiating party requests the clearing agent to process the payment.  Usage: This is the date on which the debtor&#39;s account is to be debited.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00</value>
        [DataMember(Name="RequestedExecutionDateTime", EmitDefaultValue=false)]
        public DateTimeOffset RequestedExecutionDateTime { get; set; }

        /// <summary>
        /// User community specific instrument. Usage: This element is used to specify a local instrument, local clearing option and/or further qualify the service or service level.
        /// </summary>
        /// <value>User community specific instrument. Usage: This element is used to specify a local instrument, local clearing option and/or further qualify the service or service level.</value>
        [DataMember(Name="LocalInstrument", EmitDefaultValue=false)]
        public string LocalInstrument { get; set; }

        /// <summary>
        /// Gets or Sets DebtorAccount
        /// </summary>
        [DataMember(Name="DebtorAccount", EmitDefaultValue=false)]
        public OBWriteDomestic2DataInitiationDebtorAccount DebtorAccount { get; set; }

        /// <summary>
        /// Gets or Sets RemittanceInformation
        /// </summary>
        [DataMember(Name="RemittanceInformation", EmitDefaultValue=false)]
        public OBWriteDomestic2DataInitiationRemittanceInformation RemittanceInformation { get; set; }

        /// <summary>
        /// Additional information that can not be captured in the structured fields and/or any other specific block.
        /// </summary>
        /// <value>Additional information that can not be captured in the structured fields and/or any other specific block.</value>
        [DataMember(Name="SupplementaryData", EmitDefaultValue=false)]
        public Dictionary<string, Object> SupplementaryData { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OBWriteFile2DataInitiation {\n");
            sb.Append("  FileType: ").Append(FileType).Append("\n");
            sb.Append("  FileHash: ").Append(FileHash).Append("\n");
            sb.Append("  FileReference: ").Append(FileReference).Append("\n");
            sb.Append("  NumberOfTransactions: ").Append(NumberOfTransactions).Append("\n");
            sb.Append("  ControlSum: ").Append(ControlSum).Append("\n");
            sb.Append("  RequestedExecutionDateTime: ").Append(RequestedExecutionDateTime).Append("\n");
            sb.Append("  LocalInstrument: ").Append(LocalInstrument).Append("\n");
            sb.Append("  DebtorAccount: ").Append(DebtorAccount).Append("\n");
            sb.Append("  RemittanceInformation: ").Append(RemittanceInformation).Append("\n");
            sb.Append("  SupplementaryData: ").Append(SupplementaryData).Append("\n");
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
            return this.Equals(input as OBWriteFile2DataInitiation);
        }

        /// <summary>
        /// Returns true if OBWriteFile2DataInitiation instances are equal
        /// </summary>
        /// <param name="input">Instance of OBWriteFile2DataInitiation to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBWriteFile2DataInitiation input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.FileType == input.FileType ||
                    (this.FileType != null &&
                    this.FileType.Equals(input.FileType))
                ) && 
                (
                    this.FileHash == input.FileHash ||
                    (this.FileHash != null &&
                    this.FileHash.Equals(input.FileHash))
                ) && 
                (
                    this.FileReference == input.FileReference ||
                    (this.FileReference != null &&
                    this.FileReference.Equals(input.FileReference))
                ) && 
                (
                    this.NumberOfTransactions == input.NumberOfTransactions ||
                    (this.NumberOfTransactions != null &&
                    this.NumberOfTransactions.Equals(input.NumberOfTransactions))
                ) && 
                (
                    this.ControlSum == input.ControlSum ||
                    this.ControlSum.Equals(input.ControlSum)
                ) && 
                (
                    this.RequestedExecutionDateTime == input.RequestedExecutionDateTime ||
                    (this.RequestedExecutionDateTime != null &&
                    this.RequestedExecutionDateTime.Equals(input.RequestedExecutionDateTime))
                ) && 
                (
                    this.LocalInstrument == input.LocalInstrument ||
                    (this.LocalInstrument != null &&
                    this.LocalInstrument.Equals(input.LocalInstrument))
                ) && 
                (
                    this.DebtorAccount == input.DebtorAccount ||
                    (this.DebtorAccount != null &&
                    this.DebtorAccount.Equals(input.DebtorAccount))
                ) && 
                (
                    this.RemittanceInformation == input.RemittanceInformation ||
                    (this.RemittanceInformation != null &&
                    this.RemittanceInformation.Equals(input.RemittanceInformation))
                ) && 
                (
                    this.SupplementaryData == input.SupplementaryData ||
                    this.SupplementaryData != null &&
                    input.SupplementaryData != null &&
                    this.SupplementaryData.SequenceEqual(input.SupplementaryData)
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
                if (this.FileType != null)
                    hashCode = hashCode * 59 + this.FileType.GetHashCode();
                if (this.FileHash != null)
                    hashCode = hashCode * 59 + this.FileHash.GetHashCode();
                if (this.FileReference != null)
                    hashCode = hashCode * 59 + this.FileReference.GetHashCode();
                if (this.NumberOfTransactions != null)
                    hashCode = hashCode * 59 + this.NumberOfTransactions.GetHashCode();
                hashCode = hashCode * 59 + this.ControlSum.GetHashCode();
                if (this.RequestedExecutionDateTime != null)
                    hashCode = hashCode * 59 + this.RequestedExecutionDateTime.GetHashCode();
                if (this.LocalInstrument != null)
                    hashCode = hashCode * 59 + this.LocalInstrument.GetHashCode();
                if (this.DebtorAccount != null)
                    hashCode = hashCode * 59 + this.DebtorAccount.GetHashCode();
                if (this.RemittanceInformation != null)
                    hashCode = hashCode * 59 + this.RemittanceInformation.GetHashCode();
                if (this.SupplementaryData != null)
                    hashCode = hashCode * 59 + this.SupplementaryData.GetHashCode();
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
            // FileHash (string) maxLength
            if(this.FileHash != null && this.FileHash.Length > 44)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for FileHash, length must be less than 44.", new [] { "FileHash" });
            }

            // FileHash (string) minLength
            if(this.FileHash != null && this.FileHash.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for FileHash, length must be greater than 1.", new [] { "FileHash" });
            }

            // FileReference (string) maxLength
            if(this.FileReference != null && this.FileReference.Length > 40)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for FileReference, length must be less than 40.", new [] { "FileReference" });
            }

            // FileReference (string) minLength
            if(this.FileReference != null && this.FileReference.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for FileReference, length must be greater than 1.", new [] { "FileReference" });
            }

            // NumberOfTransactions (string) pattern
            Regex regexNumberOfTransactions = new Regex(@"[0-9]{1,15}", RegexOptions.CultureInvariant);
            if (false == regexNumberOfTransactions.Match(this.NumberOfTransactions).Success)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for NumberOfTransactions, must match a pattern of " + regexNumberOfTransactions, new [] { "NumberOfTransactions" });
            }

            yield break;
        }
    }

}
