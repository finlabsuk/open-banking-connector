using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation
{
    /// <summary>
    ///     Information supplied to enable the matching of an entry with the items that the transfer is intended to settle,
    ///     such as commercial invoices in an accounts&#39; receivable system.
    /// </summary>
    [OpenBankingEquivalent(typeof(OBRemittanceInformation1))]
    [OpenBankingEquivalent(typeof(OBWriteFile2DataInitiationRemittanceInformation))]
    [PersistenceEquivalent(typeof(Persistent.PaymentInitiation.OBWriteDomesticDataInitiationRemittanceInformation))]
    public class OBWriteDomesticDataInitiationRemittanceInformation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticDataInitiationRemittanceInformation" /> class.
        /// </summary>
        public OBWriteDomesticDataInitiationRemittanceInformation()
        {
        }

        /// <summary>
        ///     Information supplied to enable the matching/reconciliation of an entry with the items that the payment is intended
        ///     to settle, such as commercial invoices in an accounts&#39; receivable system, in an unstructured form.
        /// </summary>
        /// <value>
        ///     Information supplied to enable the matching/reconciliation of an entry with the items that the payment is
        ///     intended to settle, such as commercial invoices in an accounts&#39; receivable system, in an unstructured form.
        /// </value>
        [JsonProperty("unstructured")]
        public string Unstructured { get; set; }

        /// <summary>
        ///     Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction. Usage: If
        ///     available, the initiating party should provide this reference in the structured remittance information, to enable
        ///     reconciliation by the creditor upon receipt of the amount of money. If the business context requires the use of a
        ///     creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end
        ///     chain, the creditor&#39;s reference or payment remittance identification should be quoted in the end-to-end
        ///     transaction identification. OB: The Faster Payments Scheme can only accept 18 characters for the
        ///     ReferenceInformation field - which is where this ISO field will be mapped.
        /// </summary>
        /// <value>
        ///     Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction. Usage: If
        ///     available, the initiating party should provide this reference in the structured remittance information, to enable
        ///     reconciliation by the creditor upon receipt of the amount of money. If the business context requires the use of a
        ///     creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end
        ///     chain, the creditor&#39;s reference or payment remittance identification should be quoted in the end-to-end
        ///     transaction identification. OB: The Faster Payments Scheme can only accept 18 characters for the
        ///     ReferenceInformation field - which is where this ISO field will be mapped.
        /// </value>
        [JsonProperty("reference")]
        public string Reference { get; set; }

    }
}