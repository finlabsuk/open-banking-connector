// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Defines headers for GetFilePaymentsFilePaymentId operation.
    /// </summary>
    public partial class GetFilePaymentsFilePaymentIdHeaders
    {
        /// <summary>
        /// Initializes a new instance of the
        /// GetFilePaymentsFilePaymentIdHeaders class.
        /// </summary>
        public GetFilePaymentsFilePaymentIdHeaders()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// GetFilePaymentsFilePaymentIdHeaders class.
        /// </summary>
        /// <param name="xFapiInteractionId">An RFC4122 UID used as a
        /// correlation id.</param>
        /// <param name="xJwsSignature">Header containing a detached JWS
        /// signature of the body of the payload.
        /// </param>
        /// <param name="retryAfter">Number in seconds to wait</param>
        public GetFilePaymentsFilePaymentIdHeaders(string xFapiInteractionId = default(string), string xJwsSignature = default(string), int? retryAfter = default(int?))
        {
            XFapiInteractionId = xFapiInteractionId;
            XJwsSignature = xJwsSignature;
            RetryAfter = retryAfter;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets an RFC4122 UID used as a correlation id.
        /// </summary>
        [JsonProperty(PropertyName = "x-fapi-interaction-id")]
        public string XFapiInteractionId { get; set; }

        /// <summary>
        /// Gets or sets header containing a detached JWS signature of the body
        /// of the payload.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "x-jws-signature")]
        public string XJwsSignature { get; set; }

        /// <summary>
        /// Gets or sets number in seconds to wait
        /// </summary>
        [JsonProperty(PropertyName = "Retry-After")]
        public int? RetryAfter { get; set; }

    }
}
