// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p2.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// The authorisation type request from the TPP.
    /// </summary>
    public partial class OBWriteInternationalScheduledConsent3DataAuthorisation
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsent3DataAuthorisation class.
        /// </summary>
        public OBWriteInternationalScheduledConsent3DataAuthorisation()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsent3DataAuthorisation class.
        /// </summary>
        /// <param name="authorisationType">Type of authorisation flow
        /// requested. Possible values include: 'Any', 'Single'</param>
        /// <param name="completionDateTime">Date and time at which the
        /// requested authorisation flow must be completed.All dates in the
        /// JSON payloads are represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00</param>
        public OBWriteInternationalScheduledConsent3DataAuthorisation(OBWriteInternationalScheduledConsent3DataAuthorisationAuthorisationTypeEnum authorisationType, System.DateTimeOffset? completionDateTime = default(System.DateTimeOffset?))
        {
            AuthorisationType = authorisationType;
            CompletionDateTime = completionDateTime;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets type of authorisation flow requested. Possible values
        /// include: 'Any', 'Single'
        /// </summary>
        [JsonProperty(PropertyName = "AuthorisationType")]
        public OBWriteInternationalScheduledConsent3DataAuthorisationAuthorisationTypeEnum AuthorisationType { get; set; }

        /// <summary>
        /// Gets or sets date and time at which the requested authorisation
        /// flow must be completed.All dates in the JSON payloads are
        /// represented in ISO 8601 date-time format.
        /// All date-time fields in responses must include the timezone. An
        /// example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        [JsonProperty(PropertyName = "CompletionDateTime")]
        public System.DateTimeOffset? CompletionDateTime { get; set; }

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
