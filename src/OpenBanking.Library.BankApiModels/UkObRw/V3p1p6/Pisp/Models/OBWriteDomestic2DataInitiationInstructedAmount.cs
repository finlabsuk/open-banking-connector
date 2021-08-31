// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Amount of money to be moved between the debtor and creditor, before
    /// deduction of charges, expressed in the currency as ordered by the
    /// initiating party.
    /// Usage: This amount has to be transported unchanged through the
    /// transaction chain.
    /// </summary>
    [TargetApiEquivalent(typeof(V3p1p4.Pisp.Models.OBWriteDomestic2DataInitiationInstructedAmount))]
    [SourceApiEquivalent(typeof(OBWriteDomesticConsent4DataInitiationInstructedAmount))]
    public partial class OBWriteDomestic2DataInitiationInstructedAmount
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomestic2DataInitiationInstructedAmount class.
        /// </summary>
        public OBWriteDomestic2DataInitiationInstructedAmount()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomestic2DataInitiationInstructedAmount class.
        /// </summary>
        public OBWriteDomestic2DataInitiationInstructedAmount(string amount, string currency)
        {
            Amount = amount;
            Currency = currency;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Amount")]
        public string Amount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Amount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Amount");
            }
            if (Currency == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Currency");
            }
        }
    }
}
