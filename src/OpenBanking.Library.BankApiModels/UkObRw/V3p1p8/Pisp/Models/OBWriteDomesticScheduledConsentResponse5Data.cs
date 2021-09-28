// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class OBWriteDomesticScheduledConsentResponse5Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticScheduledConsentResponse5Data class.
        /// </summary>
        public OBWriteDomesticScheduledConsentResponse5Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticScheduledConsentResponse5Data class.
        /// </summary>
        /// <param name="consentId">OB: Unique identification as assigned by
        /// the ASPSP to uniquely identify the consent resource.</param>
        /// <param name="creationDateTime">Date and time at which the resource
        /// was created.All dates in the JSON payloads are represented in ISO
        /// 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="status">Specifies the status of consent resource in
        /// code form. Possible values include: 'Authorised',
        /// 'AwaitingAuthorisation', 'Consumed', 'Rejected'</param>
        /// <param name="statusUpdateDateTime">Date and time at which the
        /// consent resource status was updated.All dates in the JSON payloads
        /// are represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="initiation">The Initiation payload is sent by the
        /// initiating party to the ASPSP. It is used to request movement of
        /// funds from the debtor account to a creditor for a single scheduled
        /// domestic payment.</param>
        /// <param name="readRefundAccount">Specifies to share the refund
        /// account details with PISP. Possible values include: 'No',
        /// 'Yes'</param>
        /// <param name="cutOffDateTime">Specified cut-off date and time for
        /// the payment consent.All dates in the JSON payloads are represented
        /// in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="expectedExecutionDateTime">Expected execution date and
        /// time for the payment resource.All dates in the JSON payloads are
        /// represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="expectedSettlementDateTime">Expected settlement date
        /// and time for the payment resource.All dates in the JSON payloads
        /// are represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="authorisation">The authorisation type request from the
        /// TPP.</param>
        public OBWriteDomesticScheduledConsentResponse5Data(string consentId, System.DateTimeOffset creationDateTime, OBWriteDomesticScheduledConsentResponse5DataStatusEnum status, System.DateTimeOffset statusUpdateDateTime, OBWriteDomesticScheduledConsentResponse5DataInitiation initiation, OBWriteDomesticScheduledConsentResponse5DataReadRefundAccountEnum? readRefundAccount = default(OBWriteDomesticScheduledConsentResponse5DataReadRefundAccountEnum?), System.DateTimeOffset? cutOffDateTime = default(System.DateTimeOffset?), System.DateTimeOffset? expectedExecutionDateTime = default(System.DateTimeOffset?), System.DateTimeOffset? expectedSettlementDateTime = default(System.DateTimeOffset?), IList<OBWriteDomesticScheduledConsentResponse5DataChargesItem> charges = default(IList<OBWriteDomesticScheduledConsentResponse5DataChargesItem>), OBWriteDomesticScheduledConsentResponse5DataAuthorisation authorisation = default(OBWriteDomesticScheduledConsentResponse5DataAuthorisation), OBSCASupportData1 sCASupportData = default(OBSCASupportData1), OBCashAccountDebtor4 debtor = default(OBCashAccountDebtor4))
        {
            ConsentId = consentId;
            CreationDateTime = creationDateTime;
            Status = status;
            StatusUpdateDateTime = statusUpdateDateTime;
            ReadRefundAccount = readRefundAccount;
            CutOffDateTime = cutOffDateTime;
            ExpectedExecutionDateTime = expectedExecutionDateTime;
            ExpectedSettlementDateTime = expectedSettlementDateTime;
            Charges = charges;
            Initiation = initiation;
            Authorisation = authorisation;
            SCASupportData = sCASupportData;
            Debtor = debtor;
            CustomInit();
        }
        /// <summary>
        /// Static constructor for OBWriteDomesticScheduledConsentResponse5Data
        /// class.
        /// </summary>
        static OBWriteDomesticScheduledConsentResponse5Data()
        {
            Permission = "Create";
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets OB: Unique identification as assigned by the ASPSP to
        /// uniquely identify the consent resource.
        /// </summary>
        [JsonProperty(PropertyName = "ConsentId")]
        public string ConsentId { get; set; }

        /// <summary>
        /// Gets or sets date and time at which the resource was created.All
        /// dates in the JSON payloads are represented in ISO 8601 date-time
        /// format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "CreationDateTime")]
        public System.DateTimeOffset CreationDateTime { get; set; }

        /// <summary>
        /// Gets or sets specifies the status of consent resource in code form.
        /// Possible values include: 'Authorised', 'AwaitingAuthorisation',
        /// 'Consumed', 'Rejected'
        /// </summary>
        [JsonProperty(PropertyName = "Status")]
        public OBWriteDomesticScheduledConsentResponse5DataStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or sets date and time at which the consent resource status was
        /// updated.All dates in the JSON payloads are represented in ISO 8601
        /// date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "StatusUpdateDateTime")]
        public System.DateTimeOffset StatusUpdateDateTime { get; set; }

        /// <summary>
        /// Gets or sets specifies to share the refund account details with
        /// PISP. Possible values include: 'No', 'Yes'
        /// </summary>
        [JsonProperty(PropertyName = "ReadRefundAccount")]
        public OBWriteDomesticScheduledConsentResponse5DataReadRefundAccountEnum? ReadRefundAccount { get; set; }

        /// <summary>
        /// Gets or sets specified cut-off date and time for the payment
        /// consent.All dates in the JSON payloads are represented in ISO 8601
        /// date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "CutOffDateTime")]
        public System.DateTimeOffset? CutOffDateTime { get; set; }

        /// <summary>
        /// Gets or sets expected execution date and time for the payment
        /// resource.All dates in the JSON payloads are represented in ISO 8601
        /// date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "ExpectedExecutionDateTime")]
        public System.DateTimeOffset? ExpectedExecutionDateTime { get; set; }

        /// <summary>
        /// Gets or sets expected settlement date and time for the payment
        /// resource.All dates in the JSON payloads are represented in ISO 8601
        /// date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "ExpectedSettlementDateTime")]
        public System.DateTimeOffset? ExpectedSettlementDateTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Charges")]
        public IList<OBWriteDomesticScheduledConsentResponse5DataChargesItem> Charges { get; set; }

        /// <summary>
        /// Gets or sets the Initiation payload is sent by the initiating party
        /// to the ASPSP. It is used to request movement of funds from the
        /// debtor account to a creditor for a single scheduled domestic
        /// payment.
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBWriteDomesticScheduledConsentResponse5DataInitiation Initiation { get; set; }

        /// <summary>
        /// Gets or sets the authorisation type request from the TPP.
        /// </summary>
        [JsonProperty(PropertyName = "Authorisation")]
        public OBWriteDomesticScheduledConsentResponse5DataAuthorisation Authorisation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SCASupportData")]
        public OBSCASupportData1 SCASupportData { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Debtor")]
        public OBCashAccountDebtor4 Debtor { get; set; }

        /// <summary>
        /// Specifies the Open Banking service request types.
        /// </summary>
        [JsonProperty(PropertyName = "Permission")]
        public static string Permission { get; private set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ConsentId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ConsentId");
            }
            if (Initiation == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Initiation");
            }
            if (ConsentId != null)
            {
                if (ConsentId.Length > 128)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "ConsentId", 128);
                }
                if (ConsentId.Length < 1)
                {
                    throw new ValidationException(ValidationRules.MinLength, "ConsentId", 1);
                }
            }
            if (Charges != null)
            {
                foreach (var element in Charges)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (Initiation != null)
            {
                Initiation.Validate();
            }
            if (Authorisation != null)
            {
                Authorisation.Validate();
            }
            if (SCASupportData != null)
            {
                SCASupportData.Validate();
            }
        }
    }
}
