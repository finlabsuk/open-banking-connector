// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    [OpenBankingEquivalent(typeof(OBWriteInternationalConsent3DataInitiationCreditor))]
    [SourceApiEquivalent(typeof(OBWriteInternationalConsent3DataInitiationCreditor))]
    public class WriteInternationalConsentDataInitiationCreditor
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("postalAddress")]
        public OBPostalAddress PostalAddress { get; set; }
    }
}
