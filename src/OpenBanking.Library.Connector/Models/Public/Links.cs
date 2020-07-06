// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    [OpenBankingEquivalent(typeof(ObModels.PaymentInitiation.V3p1p1.Model.Links))]
    [SourceApiEquivalent(typeof(ObModels.PaymentInitiation.V3p1p1.Model.Links))]
    public class Links
    {
        /// <summary>
        ///     Gets or Sets Self
        /// </summary>
        [JsonProperty("Self")]
        public string Self { get; set; }

        /// <summary>
        ///     Gets or Sets First
        /// </summary>
        [JsonProperty("First")]
        public string First { get; set; }

        /// <summary>
        ///     Gets or Sets Prev
        /// </summary>
        [JsonProperty("Prev")]
        public string Prev { get; set; }

        /// <summary>
        ///     Gets or Sets Next
        /// </summary>
        [JsonProperty("Next")]
        public string Next { get; set; }

        /// <summary>
        ///     Gets or Sets Last
        /// </summary>
        [JsonProperty("Last")]
        public string Last { get; set; }
    }
}
