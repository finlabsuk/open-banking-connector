// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class OBWriteInternationalResponse4DataRefund
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalResponse4DataRefund class.
        /// </summary>
        public OBWriteInternationalResponse4DataRefund()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalResponse4DataRefund class.
        /// </summary>
        /// <param name="account">Provides the details to identify an
        /// account.</param>
        /// <param name="creditor">Set of elements used to identify a person or
        /// an organisation.</param>
        /// <param name="agent">Set of elements used to uniquely and
        /// unambiguously identify a financial institution or a branch of a
        /// financial institution.</param>
        public OBWriteInternationalResponse4DataRefund(OBWriteInternationalResponse4DataRefundAccount account, OBWriteInternationalResponse4DataRefundCreditor creditor = default(OBWriteInternationalResponse4DataRefundCreditor), OBWriteInternationalResponse4DataRefundAgent agent = default(OBWriteInternationalResponse4DataRefundAgent))
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
        public OBWriteInternationalResponse4DataRefundCreditor Creditor { get; set; }

        /// <summary>
        /// Gets or sets set of elements used to uniquely and unambiguously
        /// identify a financial institution or a branch of a financial
        /// institution.
        /// </summary>
        [JsonProperty(PropertyName = "Agent")]
        public OBWriteInternationalResponse4DataRefundAgent Agent { get; set; }

        /// <summary>
        /// Gets or sets provides the details to identify an account.
        /// </summary>
        [JsonProperty(PropertyName = "Account")]
        public OBWriteInternationalResponse4DataRefundAccount Account { get; set; }

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