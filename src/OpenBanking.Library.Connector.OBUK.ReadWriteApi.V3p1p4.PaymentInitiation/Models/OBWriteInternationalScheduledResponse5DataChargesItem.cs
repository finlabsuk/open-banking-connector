// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p4.PaymentInitiation.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Set of elements used to provide details of a charge for the payment
    /// initiation.
    /// </summary>
    public partial class OBWriteInternationalScheduledResponse5DataChargesItem
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledResponse5DataChargesItem class.
        /// </summary>
        public OBWriteInternationalScheduledResponse5DataChargesItem()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledResponse5DataChargesItem class.
        /// </summary>
        /// <param name="chargeBearer">Possible values include:
        /// 'BorneByCreditor', 'BorneByDebtor', 'FollowingServiceLevel',
        /// 'Shared'</param>
        public OBWriteInternationalScheduledResponse5DataChargesItem(OBChargeBearerType1CodeEnum chargeBearer, string type, OBActiveOrHistoricCurrencyAndAmount amount)
        {
            ChargeBearer = chargeBearer;
            Type = type;
            Amount = amount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'BorneByCreditor',
        /// 'BorneByDebtor', 'FollowingServiceLevel', 'Shared'
        /// </summary>
        [JsonProperty(PropertyName = "ChargeBearer")]
        public OBChargeBearerType1CodeEnum ChargeBearer { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Amount")]
        public OBActiveOrHistoricCurrencyAndAmount Amount { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Type == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Type");
            }
            if (Amount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Amount");
            }
            if (Amount != null)
            {
                Amount.Validate();
            }
        }
    }
}
