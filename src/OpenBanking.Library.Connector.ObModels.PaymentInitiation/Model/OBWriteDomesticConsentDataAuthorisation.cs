// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    /// <summary>
    ///     The authorisation type request from the TPP.
    /// </summary>
    [OpenBankingEquivalent(typeof(OBAuthorisation1))]
    [OpenBankingEquivalent(typeof(OBWriteFileConsent3DataAuthorisation))]
    public class OBWriteDomesticConsentDataAuthorisation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticConsentDataAuthorisation" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticConsentDataAuthorisation() { }


        /// <summary>
        ///     Type of authorisation flow requested.
        /// </summary>
        /// <value>Type of authorisation flow requested.</value>
        [JsonProperty("authorisationType")]
        public AuthorisationType AuthorisationType { get; set; }

        /// <summary>
        ///     Date and time at which the requested authorisation flow must be completed.All dates in the JSON payloads are
        ///     represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example
        ///     is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>
        ///     Date and time at which the requested authorisation flow must be completed.All dates in the JSON payloads are
        ///     represented in ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example
        ///     is below: 2017-04-05T10:43:07+00:00
        /// </value>
        [JsonProperty("completionDateTime")]
        public DateTime CompletionDateTime { get; set; }
    }
}
