// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p2.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The Initiation payload is sent by the initiating party to the ASPSP. It
    /// is used to request movement of funds from the debtor account to a
    /// creditor for a single domestic payment.
    /// </summary>
    public partial class OBWriteDomestic2DataInitiation
    {
        /// <summary>
        /// Initializes a new instance of the OBWriteDomestic2DataInitiation
        /// class.
        /// </summary>
        public OBWriteDomestic2DataInitiation()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBWriteDomestic2DataInitiation
        /// class.
        /// </summary>
        /// <param name="instructionIdentification">Unique identification as
        /// assigned by an instructing party for an instructed party to
        /// unambiguously identify the instruction.
        /// Usage: the  instruction identification is a point to point
        /// reference that can be used between the instructing party and the
        /// instructed party to refer to the individual instruction. It can be
        /// included in several messages related to the instruction.</param>
        /// <param name="endToEndIdentification">Unique identification assigned
        /// by the initiating party to unambiguously identify the transaction.
        /// This identification is passed on, unchanged, throughout the entire
        /// end-to-end chain.
        /// Usage: The end-to-end identification can be used for reconciliation
        /// or to link tasks relating to the transaction. It can be included in
        /// several messages related to the transaction.
        /// OB: The Faster Payments Scheme can only access 31 characters for
        /// the EndToEndIdentification field.</param>
        /// <param name="instructedAmount">Amount of money to be moved between
        /// the debtor and creditor, before deduction of charges, expressed in
        /// the currency as ordered by the initiating party.
        /// Usage: This amount has to be transported unchanged through the
        /// transaction chain.</param>
        /// <param name="creditorAccount">Unambiguous identification of the
        /// account of the creditor to which a credit entry will be posted as a
        /// result of the payment transaction.</param>
        /// <param name="debtorAccount">Unambiguous identification of the
        /// account of the debtor to which a debit entry will be made as a
        /// result of the transaction.</param>
        /// <param name="remittanceInformation">Information supplied to enable
        /// the matching of an entry with the items that the transfer is
        /// intended to settle, such as commercial invoices in an accounts'
        /// receivable system.</param>
        public OBWriteDomestic2DataInitiation(string instructionIdentification, string endToEndIdentification, OBWriteDomestic2DataInitiationInstructedAmount instructedAmount, OBWriteDomestic2DataInitiationCreditorAccount creditorAccount, string localInstrument = default(string), OBWriteDomestic2DataInitiationDebtorAccount debtorAccount = default(OBWriteDomestic2DataInitiationDebtorAccount), OBPostalAddress6 creditorPostalAddress = default(OBPostalAddress6), OBWriteDomestic2DataInitiationRemittanceInformation remittanceInformation = default(OBWriteDomestic2DataInitiationRemittanceInformation), IDictionary<string, object> supplementaryData = default(IDictionary<string, object>))
        {
            InstructionIdentification = instructionIdentification;
            EndToEndIdentification = endToEndIdentification;
            LocalInstrument = localInstrument;
            InstructedAmount = instructedAmount;
            DebtorAccount = debtorAccount;
            CreditorAccount = creditorAccount;
            CreditorPostalAddress = creditorPostalAddress;
            RemittanceInformation = remittanceInformation;
            SupplementaryData = supplementaryData;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets unique identification as assigned by an instructing
        /// party for an instructed party to unambiguously identify the
        /// instruction.
        /// Usage: the  instruction identification is a point to point
        /// reference that can be used between the instructing party and the
        /// instructed party to refer to the individual instruction. It can be
        /// included in several messages related to the instruction.
        /// </summary>
        [JsonProperty(PropertyName = "InstructionIdentification")]
        public string InstructionIdentification { get; set; }

        /// <summary>
        /// Gets or sets unique identification assigned by the initiating party
        /// to unambiguously identify the transaction. This identification is
        /// passed on, unchanged, throughout the entire end-to-end chain.
        /// Usage: The end-to-end identification can be used for reconciliation
        /// or to link tasks relating to the transaction. It can be included in
        /// several messages related to the transaction.
        /// OB: The Faster Payments Scheme can only access 31 characters for
        /// the EndToEndIdentification field.
        /// </summary>
        [JsonProperty(PropertyName = "EndToEndIdentification")]
        public string EndToEndIdentification { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LocalInstrument")]
        public string LocalInstrument { get; set; }

        /// <summary>
        /// Gets or sets amount of money to be moved between the debtor and
        /// creditor, before deduction of charges, expressed in the currency as
        /// ordered by the initiating party.
        /// Usage: This amount has to be transported unchanged through the
        /// transaction chain.
        /// </summary>
        [JsonProperty(PropertyName = "InstructedAmount")]
        public OBWriteDomestic2DataInitiationInstructedAmount InstructedAmount { get; set; }

        /// <summary>
        /// Gets or sets unambiguous identification of the account of the
        /// debtor to which a debit entry will be made as a result of the
        /// transaction.
        /// </summary>
        [JsonProperty(PropertyName = "DebtorAccount")]
        public OBWriteDomestic2DataInitiationDebtorAccount DebtorAccount { get; set; }

        /// <summary>
        /// Gets or sets unambiguous identification of the account of the
        /// creditor to which a credit entry will be posted as a result of the
        /// payment transaction.
        /// </summary>
        [JsonProperty(PropertyName = "CreditorAccount")]
        public OBWriteDomestic2DataInitiationCreditorAccount CreditorAccount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CreditorPostalAddress")]
        public OBPostalAddress6 CreditorPostalAddress { get; set; }

        /// <summary>
        /// Gets or sets information supplied to enable the matching of an
        /// entry with the items that the transfer is intended to settle, such
        /// as commercial invoices in an accounts' receivable system.
        /// </summary>
        [JsonProperty(PropertyName = "RemittanceInformation")]
        public OBWriteDomestic2DataInitiationRemittanceInformation RemittanceInformation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SupplementaryData")]
        public IDictionary<string, object> SupplementaryData { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (InstructionIdentification == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "InstructionIdentification");
            }
            if (EndToEndIdentification == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "EndToEndIdentification");
            }
            if (InstructedAmount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "InstructedAmount");
            }
            if (CreditorAccount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "CreditorAccount");
            }
            if (InstructedAmount != null)
            {
                InstructedAmount.Validate();
            }
            if (DebtorAccount != null)
            {
                DebtorAccount.Validate();
            }
            if (CreditorAccount != null)
            {
                CreditorAccount.Validate();
            }
        }
    }
}
