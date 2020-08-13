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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model
{
    /// <summary>
    /// The Initiation payload is sent by the initiating party to the ASPSP. It is used to request movement of funds from the debtor account to a creditor for a domestic standing order.
    /// </summary>
    [DataContract]
    public partial class OBWriteDomesticStandingOrder3DataInitiation :  IEquatable<OBWriteDomesticStandingOrder3DataInitiation>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteDomesticStandingOrder3DataInitiation" /> class.
        /// </summary>
        [JsonConstructor]
        protected OBWriteDomesticStandingOrder3DataInitiation() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OBWriteDomesticStandingOrder3DataInitiation" /> class.
        /// </summary>
        /// <param name="frequency">Individual Definitions: EvryDay - Every day EvryWorkgDay - Every working day IntrvlWkDay - An interval specified in weeks (01 to 09), and the day within the week (01 to 07) WkInMnthDay - A monthly interval, specifying the week of the month (01 to 05) and day within the week (01 to 07) IntrvlMnthDay - An interval specified in months (between 01 to 06, 12, 24), specifying the day within the month (-5 to -1, 1 to 31) QtrDay - Quarterly (either ENGLISH, SCOTTISH, or RECEIVED).  ENGLISH &#x3D; Paid on the 25th March, 24th June, 29th September and 25th December.  SCOTTISH &#x3D; Paid on the 2nd February, 15th May, 1st August and 11th November. RECEIVED &#x3D; Paid on the 20th March, 19th June, 24th September and 20th December.  Individual Patterns: EvryDay (ScheduleCode) EvryWorkgDay (ScheduleCode) IntrvlWkDay:IntervalInWeeks:DayInWeek (ScheduleCode + IntervalInWeeks + DayInWeek) WkInMnthDay:WeekInMonth:DayInWeek (ScheduleCode + WeekInMonth + DayInWeek) IntrvlMnthDay:IntervalInMonths:DayInMonth (ScheduleCode + IntervalInMonths + DayInMonth) QtrDay: + either (ENGLISH, SCOTTISH or RECEIVED) ScheduleCode + QuarterDay The regular expression for this element combines five smaller versions for each permitted pattern. To aid legibility - the components are presented individually here: EvryDay EvryWorkgDay IntrvlWkDay:0[1-9]:0[1-7] WkInMnthDay:0[1-5]:0[1-7] IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]) QtrDay:(ENGLISH|SCOTTISH|RECEIVED) Full Regular Expression: ^(EvryDay)$|^(EvryWorkgDay)$|^(IntrvlWkDay:0[1-9]:0[1-7])$|^(WkInMnthDay:0[1-5]:0[1-7])$|^(IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]))$|^(QtrDay:(ENGLISH|SCOTTISH|RECEIVED))$ (required).</param>
        /// <param name="reference">Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction. Usage: If available, the initiating party should provide this reference in the structured remittance information, to enable reconciliation by the creditor upon receipt of the amount of money. If the business context requires the use of a creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end chain, the creditor&#39;s reference or payment remittance identification should be quoted in the end-to-end transaction identification..</param>
        /// <param name="numberOfPayments">Number of the payments that will be made in completing this frequency sequence including any executed since the sequence start date..</param>
        /// <param name="firstPaymentDateTime">The date on which the first payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00 (required).</param>
        /// <param name="recurringPaymentDateTime">The date on which the first recurring payment for a Standing Order schedule will be made.  Usage: This must be populated only if the first recurring date is different to the first payment date.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00.</param>
        /// <param name="finalPaymentDateTime">The date on which the final payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00.</param>
        /// <param name="firstPaymentAmount">firstPaymentAmount (required).</param>
        /// <param name="recurringPaymentAmount">recurringPaymentAmount.</param>
        /// <param name="finalPaymentAmount">finalPaymentAmount.</param>
        /// <param name="debtorAccount">debtorAccount.</param>
        /// <param name="creditorAccount">creditorAccount (required).</param>
        /// <param name="supplementaryData">Additional information that can not be captured in the structured fields and/or any other specific block..</param>
        public OBWriteDomesticStandingOrder3DataInitiation(string frequency = default(string), string reference = default(string), string numberOfPayments = default(string), DateTimeOffset firstPaymentDateTime = default(DateTimeOffset), DateTimeOffset recurringPaymentDateTime = default(DateTimeOffset), DateTimeOffset finalPaymentDateTime = default(DateTimeOffset), OBWriteDomesticStandingOrder3DataInitiationFirstPaymentAmount firstPaymentAmount = default(OBWriteDomesticStandingOrder3DataInitiationFirstPaymentAmount), OBWriteDomesticStandingOrder3DataInitiationRecurringPaymentAmount recurringPaymentAmount = default(OBWriteDomesticStandingOrder3DataInitiationRecurringPaymentAmount), OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount finalPaymentAmount = default(OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount), OBWriteDomesticStandingOrder3DataInitiationDebtorAccount debtorAccount = default(OBWriteDomesticStandingOrder3DataInitiationDebtorAccount), OBWriteDomesticStandingOrder3DataInitiationCreditorAccount creditorAccount = default(OBWriteDomesticStandingOrder3DataInitiationCreditorAccount), Dictionary<string, Object> supplementaryData = default(Dictionary<string, Object>))
        {
            // to ensure "frequency" is required (not null)
            this.Frequency = frequency ?? throw new ArgumentNullException("frequency is a required property for OBWriteDomesticStandingOrder3DataInitiation and cannot be null");
            this.FirstPaymentDateTime = firstPaymentDateTime;
            // to ensure "firstPaymentAmount" is required (not null)
            this.FirstPaymentAmount = firstPaymentAmount ?? throw new ArgumentNullException("firstPaymentAmount is a required property for OBWriteDomesticStandingOrder3DataInitiation and cannot be null");
            // to ensure "creditorAccount" is required (not null)
            this.CreditorAccount = creditorAccount ?? throw new ArgumentNullException("creditorAccount is a required property for OBWriteDomesticStandingOrder3DataInitiation and cannot be null");
            this.Reference = reference;
            this.NumberOfPayments = numberOfPayments;
            this.RecurringPaymentDateTime = recurringPaymentDateTime;
            this.FinalPaymentDateTime = finalPaymentDateTime;
            this.RecurringPaymentAmount = recurringPaymentAmount;
            this.FinalPaymentAmount = finalPaymentAmount;
            this.DebtorAccount = debtorAccount;
            this.SupplementaryData = supplementaryData;
        }
        
        /// <summary>
        /// Individual Definitions: EvryDay - Every day EvryWorkgDay - Every working day IntrvlWkDay - An interval specified in weeks (01 to 09), and the day within the week (01 to 07) WkInMnthDay - A monthly interval, specifying the week of the month (01 to 05) and day within the week (01 to 07) IntrvlMnthDay - An interval specified in months (between 01 to 06, 12, 24), specifying the day within the month (-5 to -1, 1 to 31) QtrDay - Quarterly (either ENGLISH, SCOTTISH, or RECEIVED).  ENGLISH &#x3D; Paid on the 25th March, 24th June, 29th September and 25th December.  SCOTTISH &#x3D; Paid on the 2nd February, 15th May, 1st August and 11th November. RECEIVED &#x3D; Paid on the 20th March, 19th June, 24th September and 20th December.  Individual Patterns: EvryDay (ScheduleCode) EvryWorkgDay (ScheduleCode) IntrvlWkDay:IntervalInWeeks:DayInWeek (ScheduleCode + IntervalInWeeks + DayInWeek) WkInMnthDay:WeekInMonth:DayInWeek (ScheduleCode + WeekInMonth + DayInWeek) IntrvlMnthDay:IntervalInMonths:DayInMonth (ScheduleCode + IntervalInMonths + DayInMonth) QtrDay: + either (ENGLISH, SCOTTISH or RECEIVED) ScheduleCode + QuarterDay The regular expression for this element combines five smaller versions for each permitted pattern. To aid legibility - the components are presented individually here: EvryDay EvryWorkgDay IntrvlWkDay:0[1-9]:0[1-7] WkInMnthDay:0[1-5]:0[1-7] IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]) QtrDay:(ENGLISH|SCOTTISH|RECEIVED) Full Regular Expression: ^(EvryDay)$|^(EvryWorkgDay)$|^(IntrvlWkDay:0[1-9]:0[1-7])$|^(WkInMnthDay:0[1-5]:0[1-7])$|^(IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]))$|^(QtrDay:(ENGLISH|SCOTTISH|RECEIVED))$
        /// </summary>
        /// <value>Individual Definitions: EvryDay - Every day EvryWorkgDay - Every working day IntrvlWkDay - An interval specified in weeks (01 to 09), and the day within the week (01 to 07) WkInMnthDay - A monthly interval, specifying the week of the month (01 to 05) and day within the week (01 to 07) IntrvlMnthDay - An interval specified in months (between 01 to 06, 12, 24), specifying the day within the month (-5 to -1, 1 to 31) QtrDay - Quarterly (either ENGLISH, SCOTTISH, or RECEIVED).  ENGLISH &#x3D; Paid on the 25th March, 24th June, 29th September and 25th December.  SCOTTISH &#x3D; Paid on the 2nd February, 15th May, 1st August and 11th November. RECEIVED &#x3D; Paid on the 20th March, 19th June, 24th September and 20th December.  Individual Patterns: EvryDay (ScheduleCode) EvryWorkgDay (ScheduleCode) IntrvlWkDay:IntervalInWeeks:DayInWeek (ScheduleCode + IntervalInWeeks + DayInWeek) WkInMnthDay:WeekInMonth:DayInWeek (ScheduleCode + WeekInMonth + DayInWeek) IntrvlMnthDay:IntervalInMonths:DayInMonth (ScheduleCode + IntervalInMonths + DayInMonth) QtrDay: + either (ENGLISH, SCOTTISH or RECEIVED) ScheduleCode + QuarterDay The regular expression for this element combines five smaller versions for each permitted pattern. To aid legibility - the components are presented individually here: EvryDay EvryWorkgDay IntrvlWkDay:0[1-9]:0[1-7] WkInMnthDay:0[1-5]:0[1-7] IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]) QtrDay:(ENGLISH|SCOTTISH|RECEIVED) Full Regular Expression: ^(EvryDay)$|^(EvryWorkgDay)$|^(IntrvlWkDay:0[1-9]:0[1-7])$|^(WkInMnthDay:0[1-5]:0[1-7])$|^(IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]))$|^(QtrDay:(ENGLISH|SCOTTISH|RECEIVED))$</value>
        [DataMember(Name="Frequency", EmitDefaultValue=false)]
        public string Frequency { get; set; }

        /// <summary>
        /// Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction. Usage: If available, the initiating party should provide this reference in the structured remittance information, to enable reconciliation by the creditor upon receipt of the amount of money. If the business context requires the use of a creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end chain, the creditor&#39;s reference or payment remittance identification should be quoted in the end-to-end transaction identification.
        /// </summary>
        /// <value>Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction. Usage: If available, the initiating party should provide this reference in the structured remittance information, to enable reconciliation by the creditor upon receipt of the amount of money. If the business context requires the use of a creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end chain, the creditor&#39;s reference or payment remittance identification should be quoted in the end-to-end transaction identification.</value>
        [DataMember(Name="Reference", EmitDefaultValue=false)]
        public string Reference { get; set; }

        /// <summary>
        /// Number of the payments that will be made in completing this frequency sequence including any executed since the sequence start date.
        /// </summary>
        /// <value>Number of the payments that will be made in completing this frequency sequence including any executed since the sequence start date.</value>
        [DataMember(Name="NumberOfPayments", EmitDefaultValue=false)]
        public string NumberOfPayments { get; set; }

        /// <summary>
        /// The date on which the first payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>The date on which the first payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00</value>
        [DataMember(Name="FirstPaymentDateTime", EmitDefaultValue=false)]
        public DateTimeOffset FirstPaymentDateTime { get; set; }

        /// <summary>
        /// The date on which the first recurring payment for a Standing Order schedule will be made.  Usage: This must be populated only if the first recurring date is different to the first payment date.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>The date on which the first recurring payment for a Standing Order schedule will be made.  Usage: This must be populated only if the first recurring date is different to the first payment date.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00</value>
        [DataMember(Name="RecurringPaymentDateTime", EmitDefaultValue=false)]
        public DateTimeOffset RecurringPaymentDateTime { get; set; }

        /// <summary>
        /// The date on which the final payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>The date on which the final payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00</value>
        [DataMember(Name="FinalPaymentDateTime", EmitDefaultValue=false)]
        public DateTimeOffset FinalPaymentDateTime { get; set; }

        /// <summary>
        /// Gets or Sets FirstPaymentAmount
        /// </summary>
        [DataMember(Name="FirstPaymentAmount", EmitDefaultValue=false)]
        public OBWriteDomesticStandingOrder3DataInitiationFirstPaymentAmount FirstPaymentAmount { get; set; }

        /// <summary>
        /// Gets or Sets RecurringPaymentAmount
        /// </summary>
        [DataMember(Name="RecurringPaymentAmount", EmitDefaultValue=false)]
        public OBWriteDomesticStandingOrder3DataInitiationRecurringPaymentAmount RecurringPaymentAmount { get; set; }

        /// <summary>
        /// Gets or Sets FinalPaymentAmount
        /// </summary>
        [DataMember(Name="FinalPaymentAmount", EmitDefaultValue=false)]
        public OBWriteDomesticStandingOrder3DataInitiationFinalPaymentAmount FinalPaymentAmount { get; set; }

        /// <summary>
        /// Gets or Sets DebtorAccount
        /// </summary>
        [DataMember(Name="DebtorAccount", EmitDefaultValue=false)]
        public OBWriteDomesticStandingOrder3DataInitiationDebtorAccount DebtorAccount { get; set; }

        /// <summary>
        /// Gets or Sets CreditorAccount
        /// </summary>
        [DataMember(Name="CreditorAccount", EmitDefaultValue=false)]
        public OBWriteDomesticStandingOrder3DataInitiationCreditorAccount CreditorAccount { get; set; }

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
            sb.Append("class OBWriteDomesticStandingOrder3DataInitiation {\n");
            sb.Append("  Frequency: ").Append(Frequency).Append("\n");
            sb.Append("  Reference: ").Append(Reference).Append("\n");
            sb.Append("  NumberOfPayments: ").Append(NumberOfPayments).Append("\n");
            sb.Append("  FirstPaymentDateTime: ").Append(FirstPaymentDateTime).Append("\n");
            sb.Append("  RecurringPaymentDateTime: ").Append(RecurringPaymentDateTime).Append("\n");
            sb.Append("  FinalPaymentDateTime: ").Append(FinalPaymentDateTime).Append("\n");
            sb.Append("  FirstPaymentAmount: ").Append(FirstPaymentAmount).Append("\n");
            sb.Append("  RecurringPaymentAmount: ").Append(RecurringPaymentAmount).Append("\n");
            sb.Append("  FinalPaymentAmount: ").Append(FinalPaymentAmount).Append("\n");
            sb.Append("  DebtorAccount: ").Append(DebtorAccount).Append("\n");
            sb.Append("  CreditorAccount: ").Append(CreditorAccount).Append("\n");
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
            return this.Equals(input as OBWriteDomesticStandingOrder3DataInitiation);
        }

        /// <summary>
        /// Returns true if OBWriteDomesticStandingOrder3DataInitiation instances are equal
        /// </summary>
        /// <param name="input">Instance of OBWriteDomesticStandingOrder3DataInitiation to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OBWriteDomesticStandingOrder3DataInitiation input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Frequency == input.Frequency ||
                    (this.Frequency != null &&
                    this.Frequency.Equals(input.Frequency))
                ) && 
                (
                    this.Reference == input.Reference ||
                    (this.Reference != null &&
                    this.Reference.Equals(input.Reference))
                ) && 
                (
                    this.NumberOfPayments == input.NumberOfPayments ||
                    (this.NumberOfPayments != null &&
                    this.NumberOfPayments.Equals(input.NumberOfPayments))
                ) && 
                (
                    this.FirstPaymentDateTime == input.FirstPaymentDateTime ||
                    (this.FirstPaymentDateTime != null &&
                    this.FirstPaymentDateTime.Equals(input.FirstPaymentDateTime))
                ) && 
                (
                    this.RecurringPaymentDateTime == input.RecurringPaymentDateTime ||
                    (this.RecurringPaymentDateTime != null &&
                    this.RecurringPaymentDateTime.Equals(input.RecurringPaymentDateTime))
                ) && 
                (
                    this.FinalPaymentDateTime == input.FinalPaymentDateTime ||
                    (this.FinalPaymentDateTime != null &&
                    this.FinalPaymentDateTime.Equals(input.FinalPaymentDateTime))
                ) && 
                (
                    this.FirstPaymentAmount == input.FirstPaymentAmount ||
                    (this.FirstPaymentAmount != null &&
                    this.FirstPaymentAmount.Equals(input.FirstPaymentAmount))
                ) && 
                (
                    this.RecurringPaymentAmount == input.RecurringPaymentAmount ||
                    (this.RecurringPaymentAmount != null &&
                    this.RecurringPaymentAmount.Equals(input.RecurringPaymentAmount))
                ) && 
                (
                    this.FinalPaymentAmount == input.FinalPaymentAmount ||
                    (this.FinalPaymentAmount != null &&
                    this.FinalPaymentAmount.Equals(input.FinalPaymentAmount))
                ) && 
                (
                    this.DebtorAccount == input.DebtorAccount ||
                    (this.DebtorAccount != null &&
                    this.DebtorAccount.Equals(input.DebtorAccount))
                ) && 
                (
                    this.CreditorAccount == input.CreditorAccount ||
                    (this.CreditorAccount != null &&
                    this.CreditorAccount.Equals(input.CreditorAccount))
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
                if (this.Frequency != null)
                    hashCode = hashCode * 59 + this.Frequency.GetHashCode();
                if (this.Reference != null)
                    hashCode = hashCode * 59 + this.Reference.GetHashCode();
                if (this.NumberOfPayments != null)
                    hashCode = hashCode * 59 + this.NumberOfPayments.GetHashCode();
                if (this.FirstPaymentDateTime != null)
                    hashCode = hashCode * 59 + this.FirstPaymentDateTime.GetHashCode();
                if (this.RecurringPaymentDateTime != null)
                    hashCode = hashCode * 59 + this.RecurringPaymentDateTime.GetHashCode();
                if (this.FinalPaymentDateTime != null)
                    hashCode = hashCode * 59 + this.FinalPaymentDateTime.GetHashCode();
                if (this.FirstPaymentAmount != null)
                    hashCode = hashCode * 59 + this.FirstPaymentAmount.GetHashCode();
                if (this.RecurringPaymentAmount != null)
                    hashCode = hashCode * 59 + this.RecurringPaymentAmount.GetHashCode();
                if (this.FinalPaymentAmount != null)
                    hashCode = hashCode * 59 + this.FinalPaymentAmount.GetHashCode();
                if (this.DebtorAccount != null)
                    hashCode = hashCode * 59 + this.DebtorAccount.GetHashCode();
                if (this.CreditorAccount != null)
                    hashCode = hashCode * 59 + this.CreditorAccount.GetHashCode();
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
            // Frequency (string) pattern
            Regex regexFrequency = new Regex(@"^(EvryDay)$|^(EvryWorkgDay)$|^(IntrvlDay:((0[2-9])|([1-2][0-9])|3[0-1]))$|^(IntrvlWkDay:0[1-9]:0[1-7])$|^(WkInMnthDay:0[1-5]:0[1-7])$|^(IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]))$|^(QtrDay:(ENGLISH|SCOTTISH|RECEIVED))$", RegexOptions.CultureInvariant);
            if (false == regexFrequency.Match(this.Frequency).Success)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Frequency, must match a pattern of " + regexFrequency, new [] { "Frequency" });
            }

            // Reference (string) maxLength
            if(this.Reference != null && this.Reference.Length > 35)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Reference, length must be less than 35.", new [] { "Reference" });
            }

            // Reference (string) minLength
            if(this.Reference != null && this.Reference.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Reference, length must be greater than 1.", new [] { "Reference" });
            }

            // NumberOfPayments (string) maxLength
            if(this.NumberOfPayments != null && this.NumberOfPayments.Length > 35)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for NumberOfPayments, length must be less than 35.", new [] { "NumberOfPayments" });
            }

            // NumberOfPayments (string) minLength
            if(this.NumberOfPayments != null && this.NumberOfPayments.Length < 1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for NumberOfPayments, length must be greater than 1.", new [] { "NumberOfPayments" });
            }

            yield break;
        }
    }

}
