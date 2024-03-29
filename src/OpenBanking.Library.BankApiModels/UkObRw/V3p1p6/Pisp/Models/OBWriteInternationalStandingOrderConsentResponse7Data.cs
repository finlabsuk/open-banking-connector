// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class OBWriteInternationalStandingOrderConsentResponse7Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderConsentResponse7Data class.
        /// </summary>
        public OBWriteInternationalStandingOrderConsentResponse7Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderConsentResponse7Data class.
        /// </summary>
        /// <param name="consentId">OB: Unique identification as assigned by
        /// the ASPSP to uniquely identify the consent resource.</param>
        /// <param name="creationDateTime">Date and time at which the resource
        /// was created.All dates in the JSON payloads are represented in ISO
        /// 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="status">Specifies the status of resource in code form.
        /// Possible values include: 'Authorised', 'AwaitingAuthorisation',
        /// 'Consumed', 'Rejected'</param>
        /// <param name="statusUpdateDateTime">Date and time at which the
        /// resource status was updated.All dates in the JSON payloads are
        /// represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="initiation">The Initiation payload is sent by the
        /// initiating party to the ASPSP. It is used to request movement of
        /// funds from the debtor account to a creditor for an international
        /// standing order.</param>
        /// <param name="readRefundAccount">Specifies to share the refund
        /// account details with PISP. Possible values include: 'No',
        /// 'Yes'</param>
        /// <param name="cutOffDateTime">Specified cut-off date and time for
        /// the payment consent.All dates in the JSON payloads are represented
        /// in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="authorisation">The authorisation type request from the
        /// TPP.</param>
        /// <param name="sCASupportData">Supporting Data provided by TPP, when
        /// requesting SCA Exemption.</param>
        public OBWriteInternationalStandingOrderConsentResponse7Data(string consentId, System.DateTimeOffset creationDateTime, OBWriteInternationalStandingOrderConsentResponse7DataStatusEnum status, System.DateTimeOffset statusUpdateDateTime, OBWriteInternationalStandingOrderConsentResponse7DataInitiation initiation, OBWriteInternationalStandingOrderConsentResponse7DataReadRefundAccountEnum? readRefundAccount = default(OBWriteInternationalStandingOrderConsentResponse7DataReadRefundAccountEnum?), System.DateTimeOffset? cutOffDateTime = default(System.DateTimeOffset?), IList<OBWriteInternationalStandingOrderConsentResponse7DataChargesItem> charges = default(IList<OBWriteInternationalStandingOrderConsentResponse7DataChargesItem>), OBWriteInternationalStandingOrderConsentResponse7DataAuthorisation authorisation = default(OBWriteInternationalStandingOrderConsentResponse7DataAuthorisation), OBWriteInternationalStandingOrderConsentResponse7DataSCASupportData sCASupportData = default(OBWriteInternationalStandingOrderConsentResponse7DataSCASupportData), OBDebtorIdentification1 debtor = default(OBDebtorIdentification1))
        {
            ConsentId = consentId;
            CreationDateTime = creationDateTime;
            Status = status;
            StatusUpdateDateTime = statusUpdateDateTime;
            ReadRefundAccount = readRefundAccount;
            CutOffDateTime = cutOffDateTime;
            Charges = charges;
            Initiation = initiation;
            Authorisation = authorisation;
            SCASupportData = sCASupportData;
            Debtor = debtor;
            CustomInit();
        }
        /// <summary>
        /// Static constructor for
        /// OBWriteInternationalStandingOrderConsentResponse7Data class.
        /// </summary>
        static OBWriteInternationalStandingOrderConsentResponse7Data()
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
        /// Gets or sets specifies the status of resource in code form.
        /// Possible values include: 'Authorised', 'AwaitingAuthorisation',
        /// 'Consumed', 'Rejected'
        /// </summary>
        [JsonProperty(PropertyName = "Status")]
        public OBWriteInternationalStandingOrderConsentResponse7DataStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or sets date and time at which the resource status was
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
        public OBWriteInternationalStandingOrderConsentResponse7DataReadRefundAccountEnum? ReadRefundAccount { get; set; }

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
        /// </summary>
        [JsonProperty(PropertyName = "Charges")]
        public IList<OBWriteInternationalStandingOrderConsentResponse7DataChargesItem> Charges { get; set; }

        /// <summary>
        /// Gets or sets the Initiation payload is sent by the initiating party
        /// to the ASPSP. It is used to request movement of funds from the
        /// debtor account to a creditor for an international standing order.
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBWriteInternationalStandingOrderConsentResponse7DataInitiation Initiation { get; set; }

        /// <summary>
        /// Gets or sets the authorisation type request from the TPP.
        /// </summary>
        [JsonProperty(PropertyName = "Authorisation")]
        public OBWriteInternationalStandingOrderConsentResponse7DataAuthorisation Authorisation { get; set; }

        /// <summary>
        /// Gets or sets supporting Data provided by TPP, when requesting SCA
        /// Exemption.
        /// </summary>
        [JsonProperty(PropertyName = "SCASupportData")]
        public OBWriteInternationalStandingOrderConsentResponse7DataSCASupportData SCASupportData { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Debtor")]
        public OBDebtorIdentification1 Debtor { get; set; }

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
        }
    }
}
