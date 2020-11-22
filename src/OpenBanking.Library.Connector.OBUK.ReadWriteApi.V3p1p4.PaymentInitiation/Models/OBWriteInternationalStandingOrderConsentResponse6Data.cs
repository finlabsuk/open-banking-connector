// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p4.PaymentInitiation.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class OBWriteInternationalStandingOrderConsentResponse6Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderConsentResponse6Data class.
        /// </summary>
        public OBWriteInternationalStandingOrderConsentResponse6Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderConsentResponse6Data class.
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
        public OBWriteInternationalStandingOrderConsentResponse6Data(string consentId, System.DateTimeOffset creationDateTime, OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum status, System.DateTimeOffset statusUpdateDateTime, OBWriteInternationalStandingOrderConsentResponse6DataInitiation initiation, OBWriteInternationalStandingOrderConsentResponse6DataReadRefundAccountEnum? readRefundAccount = default(OBWriteInternationalStandingOrderConsentResponse6DataReadRefundAccountEnum?), System.DateTimeOffset? cutOffDateTime = default(System.DateTimeOffset?), IList<OBWriteInternationalStandingOrderConsentResponse6DataChargesItem> charges = default(IList<OBWriteInternationalStandingOrderConsentResponse6DataChargesItem>), OBWriteInternationalStandingOrderConsentResponse6DataAuthorisation authorisation = default(OBWriteInternationalStandingOrderConsentResponse6DataAuthorisation), OBWriteInternationalStandingOrderConsentResponse6DataSCASupportData sCASupportData = default(OBWriteInternationalStandingOrderConsentResponse6DataSCASupportData))
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
            CustomInit();
        }
        /// <summary>
        /// Static constructor for
        /// OBWriteInternationalStandingOrderConsentResponse6Data class.
        /// </summary>
        static OBWriteInternationalStandingOrderConsentResponse6Data()
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
        public OBWriteInternationalStandingOrderConsentResponse6DataStatusEnum Status { get; set; }

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
        public OBWriteInternationalStandingOrderConsentResponse6DataReadRefundAccountEnum? ReadRefundAccount { get; set; }

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
        public IList<OBWriteInternationalStandingOrderConsentResponse6DataChargesItem> Charges { get; set; }

        /// <summary>
        /// Gets or sets the Initiation payload is sent by the initiating party
        /// to the ASPSP. It is used to request movement of funds from the
        /// debtor account to a creditor for an international standing order.
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBWriteInternationalStandingOrderConsentResponse6DataInitiation Initiation { get; set; }

        /// <summary>
        /// Gets or sets the authorisation type request from the TPP.
        /// </summary>
        [JsonProperty(PropertyName = "Authorisation")]
        public OBWriteInternationalStandingOrderConsentResponse6DataAuthorisation Authorisation { get; set; }

        /// <summary>
        /// Gets or sets supporting Data provided by TPP, when requesting SCA
        /// Exemption.
        /// </summary>
        [JsonProperty(PropertyName = "SCASupportData")]
        public OBWriteInternationalStandingOrderConsentResponse6DataSCASupportData SCASupportData { get; set; }

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
