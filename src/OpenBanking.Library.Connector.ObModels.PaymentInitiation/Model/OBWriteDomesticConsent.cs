// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using Newtonsoft.Json;
using OBWriteDomestic2 =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomestic2;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    /// <summary>
    ///     OBWriteDomesticConsent
    /// </summary>
    [OpenBankingEquivalent(typeof(OBWriteDomesticConsent2))]
    [OpenBankingEquivalent(typeof(OBWriteDomestic2))]
    [OpenBankingEquivalent(typeof(OBWriteDomesticConsent3))]
    public class OBWriteDomesticConsent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticConsent" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticConsent() { }


        /// <summary>
        ///     OBClientProfile ID to use
        /// </summary>
        [JsonProperty("apiProfileId", Required = Required.Always)]
        public string ApiProfileId { get; set; }

        /// <summary>
        ///     Gets or Sets Data
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public OBWriteDomesticConsentData Data { get; set; }

        /// <summary>
        ///     Gets or Sets Risk
        /// </summary>
        [JsonProperty("risk", Required = Required.Default)]
        public OBRisk Risk { get; set; }
    }
}
