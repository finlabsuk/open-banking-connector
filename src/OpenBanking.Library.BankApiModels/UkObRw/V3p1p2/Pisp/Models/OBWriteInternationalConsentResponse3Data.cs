// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p2.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class OBWriteInternationalConsentResponse3Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalConsentResponse3Data class.
        /// </summary>
        public OBWriteInternationalConsentResponse3Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalConsentResponse3Data class.
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
        /// resource status was updated.All dates in the JSON payloads are
        /// represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="initiation">The Initiation payload is sent by the
        /// initiating party to the ASPSP. It is used to request movement of
        /// funds from the debtor account to a creditor for a single
        /// international payment.</param>
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
        /// <param name="exchangeRateInformation">Further detailed information
        /// on the exchange rate that has been used in the payment
        /// transaction.</param>
        /// <param name="authorisation">The authorisation type request from the
        /// TPP.</param>
        /// <param name="sCASupportData">Supporting Data provided by TPP, when
        /// requesting SCA Exemption.</param>
        public OBWriteInternationalConsentResponse3Data(string consentId, System.DateTimeOffset creationDateTime, OBWriteInternationalConsentResponse3DataStatusEnum status, System.DateTimeOffset statusUpdateDateTime, OBWriteInternationalConsentResponse3DataInitiation initiation, System.DateTimeOffset? cutOffDateTime = default(System.DateTimeOffset?), System.DateTimeOffset? expectedExecutionDateTime = default(System.DateTimeOffset?), System.DateTimeOffset? expectedSettlementDateTime = default(System.DateTimeOffset?), IList<OBWriteInternationalConsentResponse3DataChargesItem> charges = default(IList<OBWriteInternationalConsentResponse3DataChargesItem>), OBWriteInternationalConsentResponse3DataExchangeRateInformation exchangeRateInformation = default(OBWriteInternationalConsentResponse3DataExchangeRateInformation), OBWriteInternationalConsentResponse3DataAuthorisation authorisation = default(OBWriteInternationalConsentResponse3DataAuthorisation), OBWriteInternationalConsentResponse3DataSCASupportData sCASupportData = default(OBWriteInternationalConsentResponse3DataSCASupportData))
        {
            ConsentId = consentId;
            CreationDateTime = creationDateTime;
            Status = status;
            StatusUpdateDateTime = statusUpdateDateTime;
            CutOffDateTime = cutOffDateTime;
            ExpectedExecutionDateTime = expectedExecutionDateTime;
            ExpectedSettlementDateTime = expectedSettlementDateTime;
            Charges = charges;
            ExchangeRateInformation = exchangeRateInformation;
            Initiation = initiation;
            Authorisation = authorisation;
            SCASupportData = sCASupportData;
            CustomInit();
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
        public OBWriteInternationalConsentResponse3DataStatusEnum Status { get; set; }

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
        public IList<OBWriteInternationalConsentResponse3DataChargesItem> Charges { get; set; }

        /// <summary>
        /// Gets or sets further detailed information on the exchange rate that
        /// has been used in the payment transaction.
        /// </summary>
        [JsonProperty(PropertyName = "ExchangeRateInformation")]
        public OBWriteInternationalConsentResponse3DataExchangeRateInformation ExchangeRateInformation { get; set; }

        /// <summary>
        /// Gets or sets the Initiation payload is sent by the initiating party
        /// to the ASPSP. It is used to request movement of funds from the
        /// debtor account to a creditor for a single international payment.
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBWriteInternationalConsentResponse3DataInitiation Initiation { get; set; }

        /// <summary>
        /// Gets or sets the authorisation type request from the TPP.
        /// </summary>
        [JsonProperty(PropertyName = "Authorisation")]
        public OBWriteInternationalConsentResponse3DataAuthorisation Authorisation { get; set; }

        /// <summary>
        /// Gets or sets supporting Data provided by TPP, when requesting SCA
        /// Exemption.
        /// </summary>
        [JsonProperty(PropertyName = "SCASupportData")]
        public OBWriteInternationalConsentResponse3DataSCASupportData SCASupportData { get; set; }

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
            if (ExchangeRateInformation != null)
            {
                ExchangeRateInformation.Validate();
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
