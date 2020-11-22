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
    /// Defines headers for File Payments operation.
    /// </summary>
    public partial class OBC2FilePaymentsHeaders4
    {
        /// <summary>
        /// Initializes a new instance of the OBC2FilePaymentsHeaders4 class.
        /// </summary>
        public OBC2FilePaymentsHeaders4()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBC2FilePaymentsHeaders4 class.
        /// </summary>
        /// <param name="xFapiInteractionId">An RFC4122 UID used as a
        /// correlation id.</param>
        /// <param name="xJwsSignature">Header containing a detached JWS
        /// signature of the body of the payload.
        /// </param>
        /// <param name="retryAfter">Number in seconds to wait</param>
        public OBC2FilePaymentsHeaders4(string xFapiInteractionId = default(string), string xJwsSignature = default(string), int? retryAfter = default(int?))
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
