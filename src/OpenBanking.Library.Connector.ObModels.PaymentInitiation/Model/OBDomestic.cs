// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    [SourceApiEquivalent(typeof(OBDomestic2))]
    public class OBDomestic
    {
        [JsonProperty("InstructionIdentification")]
        public string InstructionIdentification { get; set; }

        /// <summary>
        ///     Unique identification assigned by the initiating party to unambiguously identify the transaction. This
        ///     identification is passed on, unchanged, throughout the entire end-to-end chain. Usage: The end-to-end
        ///     identification can be used for reconciliation or to link tasks relating to the transaction. It can be included in
        ///     several messages related to the transaction. OB: The Faster Payments Scheme can only access 31 characters for the
        ///     EndToEndIdentification field.
        /// </summary>
        /// <value>
        ///     Unique identification assigned by the initiating party to unambiguously identify the transaction. This
        ///     identification is passed on, unchanged, throughout the entire end-to-end chain. Usage: The end-to-end
        ///     identification can be used for reconciliation or to link tasks relating to the transaction. It can be included in
        ///     several messages related to the transaction. OB: The Faster Payments Scheme can only access 31 characters for the
        ///     EndToEndIdentification field.
        /// </value>
        [JsonProperty("EndToEndIdentification")]
        public string EndToEndIdentification { get; set; }

        /// <summary>
        ///     User community specific instrument. Usage: This element is used to specify a local instrument, local clearing
        ///     option and/or further qualify the service or service level.
        /// </summary>
        /// <value>
        ///     User community specific instrument. Usage: This element is used to specify a local instrument, local clearing
        ///     option and/or further qualify the service or service level.
        /// </value>
        [JsonProperty("LocalInstrument")]
        public string LocalInstrument { get; set; }

        /// <summary>
        ///     Gets or Sets InstructedAmount
        /// </summary>
        [JsonProperty("InstructedAmount")]
        public OBInternationalInstructedAmount InstructedAmount { get; set; }

        /// <summary>
        ///     Gets or Sets DebtorAccount
        /// </summary>
        [JsonProperty("DebtorAccount")]
        public OBCashAccountDebtor DebtorAccount { get; set; }

        /// <summary>
        ///     Gets or Sets CreditorAccount
        /// </summary>
        [JsonProperty("CreditorAccount")]
        public OBCashAccountCreditor CreditorAccount { get; set; }

        /// <summary>
        ///     Gets or Sets CreditorPostalAddress
        /// </summary>
        [JsonProperty("CreditorPostalAddress")]
        public OBPostalAddress CreditorPostalAddress { get; set; }

        /// <summary>
        ///     Gets or Sets RemittanceInformation
        /// </summary>
        [JsonProperty("RemittanceInformation")]
        public OBRemittanceInformation RemittanceInformation { get; set; }

        /// <summary>
        ///     Gets or Sets SupplementaryData
        /// </summary>
        [JsonProperty("SupplementaryData")]
        public OBSupplementaryData SupplementaryData { get; set; }
    }
}
