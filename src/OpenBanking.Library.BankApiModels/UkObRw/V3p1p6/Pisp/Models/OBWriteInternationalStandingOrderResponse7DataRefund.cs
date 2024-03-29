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

    public partial class OBWriteInternationalStandingOrderResponse7DataRefund
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderResponse7DataRefund class.
        /// </summary>
        public OBWriteInternationalStandingOrderResponse7DataRefund()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderResponse7DataRefund class.
        /// </summary>
        /// <param name="account">Provides the details to identify an
        /// account.</param>
        /// <param name="creditor">Set of elements used to identify a person or
        /// an organisation.</param>
        /// <param name="agent">Set of elements used to uniquely and
        /// unambiguously identify a financial institution or a branch of a
        /// financial institution.</param>
        public OBWriteInternationalStandingOrderResponse7DataRefund(OBWriteInternationalStandingOrderResponse7DataRefundAccount account, OBWriteInternationalStandingOrderResponse7DataRefundCreditor creditor = default(OBWriteInternationalStandingOrderResponse7DataRefundCreditor), OBWriteInternationalStandingOrderResponse7DataRefundAgent agent = default(OBWriteInternationalStandingOrderResponse7DataRefundAgent))
        {
            Creditor = creditor;
            Agent = agent;
            Account = account;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets set of elements used to identify a person or an
        /// organisation.
        /// </summary>
        [JsonProperty(PropertyName = "Creditor")]
        public OBWriteInternationalStandingOrderResponse7DataRefundCreditor Creditor { get; set; }

        /// <summary>
        /// Gets or sets set of elements used to uniquely and unambiguously
        /// identify a financial institution or a branch of a financial
        /// institution.
        /// </summary>
        [JsonProperty(PropertyName = "Agent")]
        public OBWriteInternationalStandingOrderResponse7DataRefundAgent Agent { get; set; }

        /// <summary>
        /// Gets or sets provides the details to identify an account.
        /// </summary>
        [JsonProperty(PropertyName = "Account")]
        public OBWriteInternationalStandingOrderResponse7DataRefundAccount Account { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Account == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Account");
            }
            if (Account != null)
            {
                Account.Validate();
            }
        }
    }
}
