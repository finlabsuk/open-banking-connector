// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Converters;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    /// <summary>
    ///     OBWriteDomesticConsentData
    /// </summary>
    [OpenBankingEquivalent(typeof(OBWriteDataDomesticConsent2))]
    [OpenBankingEquivalent(typeof(OBWriteDataDomestic2), EquivalentTypeMapper = typeof(OBWriteDataDomestic2Converter))]
    [OpenBankingEquivalent(typeof(OBWriteDomesticConsent3Data))]
    public class OBWriteDomesticConsentData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticConsentData" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticConsentData() { }

        /// <summary>
        ///     Gets or Sets Initiation
        /// </summary>
        [JsonProperty("initiation")]
        public OBWriteDomesticDataInitiation Initiation { get; set; }

        /// <summary>
        ///     Gets or Sets Authorisation
        /// </summary>
        [JsonProperty("authorisation")]
        public OBWriteDomesticConsentDataAuthorisation Authorisation { get; set; }

        /// <summary>
        ///     Gets or Sets SCASupportData
        /// </summary>
        [JsonProperty("scaSupportData", Required = Required.Default)]
        public OBWriteDomesticConsentDataSCASupportData SCASupportData { get; set; }
    }
}
