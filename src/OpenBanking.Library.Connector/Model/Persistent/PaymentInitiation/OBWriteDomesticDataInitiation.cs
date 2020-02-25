using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation
{
    /// <summary>
    ///     The Initiation payload is sent by the initiating party to the ASPSP. It is used to request movement of funds from
    ///     the debtor account to a creditor for a single domestic payment.
    /// </summary>
    public class OBWriteDomesticDataInitiation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticDataInitiation" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticDataInitiation()
        {
        }


        /// <summary>
        ///     Unique identification as assigned by an instructing party for an instructed party to unambiguously identify the
        ///     instruction. Usage: the  instruction identification is a point to point reference that can be used between the
        ///     instructing party and the instructed party to refer to the individual instruction. It can be included in several
        ///     messages related to the instruction.
        /// </summary>
        /// <value>
        ///     Unique identification as assigned by an instructing party for an instructed party to unambiguously identify the
        ///     instruction. Usage: the  instruction identification is a point to point reference that can be used between the
        ///     instructing party and the instructed party to refer to the individual instruction. It can be included in several
        ///     messages related to the instruction.
        /// </value>
        [JsonProperty("instructionIdentification")]
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
        [JsonProperty("endToEndIdentification")]
        public string EndToEndIdentification { get; set; }

        /// <summary>
        ///     User community specific instrument. Usage: This element is used to specify a local instrument, local clearing
        ///     option and/or further qualify the service or service level.
        /// </summary>
        /// <value>
        ///     User community specific instrument. Usage: This element is used to specify a local instrument, local clearing
        ///     option and/or further qualify the service or service level.
        /// </value>
        [JsonProperty("localInstrument")]
        public string LocalInstrument { get; set; }

        /// <summary>
        ///     Gets or Sets InstructedAmount
        /// </summary>
        [JsonProperty("instructedAmount")]
        public OBWriteDomesticDataInitiationInstructedAmount InstructedAmount { get; set; }

        /// <summary>
        ///     Gets or Sets DebtorAccount
        /// </summary>
        [JsonProperty("debtorAccount")]
        public OBWriteDomesticDataInitiationDebtorAccount DebtorAccount { get; set; }

        /// <summary>
        ///     Gets or Sets CreditorAccount
        /// </summary>
        [JsonProperty("creditorAccount")]
        public OBWriteDomesticDataInitiationCreditorAccount CreditorAccount { get; set; }

        /// <summary>
        ///     Gets or Sets CreditorPostalAddress
        /// </summary>
        [JsonProperty("creditorPostalAddress")]
        public OBPostalAddress CreditorPostalAddress { get; set; }

        /// <summary>
        ///     Gets or Sets RemittanceInformation
        /// </summary>
        [JsonProperty("remittanceInformation")]
        public OBWriteDomesticDataInitiationRemittanceInformation RemittanceInformation { get; set; }

        /// <summary>
        ///     Additional information that can not be captured in the structured fields and/or any other specific block.
        /// </summary>
        /// <value>Additional information that can not be captured in the structured fields and/or any other specific block.</value>
        [JsonProperty("supplementaryData")]
        public object SupplementaryData { get; set; }

        

    }
}