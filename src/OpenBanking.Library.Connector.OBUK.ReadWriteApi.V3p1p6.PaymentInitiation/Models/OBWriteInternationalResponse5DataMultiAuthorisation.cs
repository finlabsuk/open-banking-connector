// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p6.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// The multiple authorisation flow response from the ASPSP.
    /// </summary>
    public partial class OBWriteInternationalResponse5DataMultiAuthorisation
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalResponse5DataMultiAuthorisation class.
        /// </summary>
        public OBWriteInternationalResponse5DataMultiAuthorisation()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalResponse5DataMultiAuthorisation class.
        /// </summary>
        /// <param name="status">Specifies the status of the authorisation flow
        /// in code form. Possible values include: 'Authorised',
        /// 'AwaitingFurtherAuthorisation', 'Rejected'</param>
        /// <param name="numberRequired">Number of authorisations required for
        /// payment order (total required at the start of the multi
        /// authorisation journey).</param>
        /// <param name="numberReceived">Number of authorisations
        /// received.</param>
        /// <param name="lastUpdateDateTime">Last date and time at the
        /// authorisation flow was updated.All dates in the JSON payloads are
        /// represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        /// <param name="expirationDateTime">Date and time at which the
        /// requested authorisation flow must be completed.All dates in the
        /// JSON payloads are represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        public OBWriteInternationalResponse5DataMultiAuthorisation(OBWriteInternationalResponse5DataMultiAuthorisationStatusEnum status, int? numberRequired = default(int?), int? numberReceived = default(int?), System.DateTimeOffset? lastUpdateDateTime = default(System.DateTimeOffset?), System.DateTimeOffset? expirationDateTime = default(System.DateTimeOffset?))
        {
            Status = status;
            NumberRequired = numberRequired;
            NumberReceived = numberReceived;
            LastUpdateDateTime = lastUpdateDateTime;
            ExpirationDateTime = expirationDateTime;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets specifies the status of the authorisation flow in code
        /// form. Possible values include: 'Authorised',
        /// 'AwaitingFurtherAuthorisation', 'Rejected'
        /// </summary>
        [JsonProperty(PropertyName = "Status")]
        public OBWriteInternationalResponse5DataMultiAuthorisationStatusEnum Status { get; set; }

        /// <summary>
        /// Gets or sets number of authorisations required for payment order
        /// (total required at the start of the multi authorisation journey).
        /// </summary>
        [JsonProperty(PropertyName = "NumberRequired")]
        public int? NumberRequired { get; set; }

        /// <summary>
        /// Gets or sets number of authorisations received.
        /// </summary>
        [JsonProperty(PropertyName = "NumberReceived")]
        public int? NumberReceived { get; set; }

        /// <summary>
        /// Gets or sets last date and time at the authorisation flow was
        /// updated.All dates in the JSON payloads are represented in ISO 8601
        /// date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "LastUpdateDateTime")]
        public System.DateTimeOffset? LastUpdateDateTime { get; set; }

        /// <summary>
        /// Gets or sets date and time at which the requested authorisation
        /// flow must be completed.All dates in the JSON payloads are
        /// represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "ExpirationDateTime")]
        public System.DateTimeOffset? ExpirationDateTime { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
        }
    }
}
