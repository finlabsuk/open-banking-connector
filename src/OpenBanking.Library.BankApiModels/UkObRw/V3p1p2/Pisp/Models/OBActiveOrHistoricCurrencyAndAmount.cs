// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p2.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Amount of money associated with the charge type.
    /// </summary>
    public partial class OBActiveOrHistoricCurrencyAndAmount
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBActiveOrHistoricCurrencyAndAmount class.
        /// </summary>
        public OBActiveOrHistoricCurrencyAndAmount()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBActiveOrHistoricCurrencyAndAmount class.
        /// </summary>
        public OBActiveOrHistoricCurrencyAndAmount(string amount, string currency)
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