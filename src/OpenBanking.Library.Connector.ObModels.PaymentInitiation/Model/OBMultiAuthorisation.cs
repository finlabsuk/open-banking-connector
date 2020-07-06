using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    public class OBMultiAuthorisation
    {
        [JsonProperty("Status")]
        public OBExternalStatusCode Status { get; set; }

        /// <summary>
        /// Number of authorisations required for payment order (total required at the start of the multi authorisation journey).
        /// </summary>
        /// <value>Number of authorisations required for payment order (total required at the start of the multi authorisation journey).</value>
        [JsonProperty("NumberRequired")]
        public int? NumberRequired { get; set; }

        /// <summary>
        /// Number of authorisations required for payment order (total required at the start of the multi authorisation journey).
        /// </summary>
        /// <value>Number of authorisations required for payment order (total required at the start of the multi authorisation journey).</value>
        [JsonProperty("NumberReceived")]
        public int? NumberReceived { get; set; }

        /// <summary>
        /// Last date and time at the authorisation flow was updated. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>Last date and time at the authorisation flow was updated. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00</value>
        [JsonProperty("LastUpdateDateTime")]
        public DateTime? LastUpdateDateTime { get; set; }

        /// <summary>
        /// Date and time at which the requested authorisation flow must be completed. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>Date and time at which the requested authorisation flow must be completed. All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below: 2017-04-05T10:43:07+00:00</value>
        [JsonProperty("ExpirationDateTime")]
        public DateTime? ExpirationDateTime { get; set; }
    }
}
