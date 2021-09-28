// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Information supplied to enable the matching of an entry with the items
    /// that the transfer is intended to settle, such as commercial invoices in
    /// an accounts' receivable system.
    /// </summary>
    public partial class OBWriteInternationalScheduledConsentResponse6DataInitiationRemittanceInformation
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsentResponse6DataInitiationRemittanceInformation
        /// class.
        /// </summary>
        public OBWriteInternationalScheduledConsentResponse6DataInitiationRemittanceInformation()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsentResponse6DataInitiationRemittanceInformation
        /// class.
        /// </summary>
        /// <param name="unstructured">Information supplied to enable the
        /// matching/reconciliation of an entry with the items that the payment
        /// is intended to settle, such as commercial invoices in an accounts'
        /// receivable system, in an unstructured form.</param>
        /// <param name="reference">Unique reference, as assigned by the
        /// creditor, to unambiguously refer to the payment transaction.
        /// Usage: If available, the initiating party should provide this
        /// reference in the structured remittance information, to enable
        /// reconciliation by the creditor upon receipt of the amount of money.
        /// If the business context requires the use of a creditor reference or
        /// a payment remit identification, and only one identifier can be
        /// passed through the end-to-end chain, the creditor's reference or
        /// payment remittance identification should be quoted in the
        /// end-to-end transaction identification.
        /// OB: The Faster Payments Scheme can only accept 18 characters for
        /// the ReferenceInformation field - which is where this ISO field will
        /// be mapped.</param>
        public OBWriteInternationalScheduledConsentResponse6DataInitiationRemittanceInformation(string unstructured = default(string), string reference = default(string))
        {
            Unstructured = unstructured;
            Reference = reference;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets information supplied to enable the
        /// matching/reconciliation of an entry with the items that the payment
        /// is intended to settle, such as commercial invoices in an accounts'
        /// receivable system, in an unstructured form.
        /// </summary>
        [JsonProperty(PropertyName = "Unstructured")]
        public string Unstructured { get; set; }

        /// <summary>
        /// Gets or sets unique reference, as assigned by the creditor, to
        /// unambiguously refer to the payment transaction.
        /// Usage: If available, the initiating party should provide this
        /// reference in the structured remittance information, to enable
        /// reconciliation by the creditor upon receipt of the amount of money.
        /// If the business context requires the use of a creditor reference or
        /// a payment remit identification, and only one identifier can be
        /// passed through the end-to-end chain, the creditor's reference or
        /// payment remittance identification should be quoted in the
        /// end-to-end transaction identification.
        /// OB: The Faster Payments Scheme can only accept 18 characters for
        /// the ReferenceInformation field - which is where this ISO field will
        /// be mapped.
        /// </summary>
        [JsonProperty(PropertyName = "Reference")]
        public string Reference { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Unstructured != null)
            {
                if (Unstructured.Length > 140)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Unstructured", 140);
                }
                if (Unstructured.Length < 1)
                {
                    throw new ValidationException(ValidationRules.MinLength, "Unstructured", 1);
                }
            }
            if (Reference != null)
            {
                if (Reference.Length > 35)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "Reference", 35);
                }
                if (Reference.Length < 1)
                {
                    throw new ValidationException(ValidationRules.MinLength, "Reference", 1);
                }
            }
        }
    }
}
