// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class OBDomesticVRPConsentRequestData
    {
        /// <summary>
        /// Initializes a new instance of the OBDomesticVRPConsentRequestData
        /// class.
        /// </summary>
        public OBDomesticVRPConsentRequestData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBDomesticVRPConsentRequestData
        /// class.
        /// </summary>
        /// <param name="readRefundAccount">Indicates whether information about
        /// RefundAccount should be included in the payment response.
        /// . Possible values include: 'Yes', 'No'</param>
        public OBDomesticVRPConsentRequestData(OBDomesticVRPControlParameters controlParameters, OBDomesticVRPInitiation initiation, OBDomesticVRPConsentRequestDataReadRefundAccountEnum? readRefundAccount = default(OBDomesticVRPConsentRequestDataReadRefundAccountEnum?))
        {
            ReadRefundAccount = readRefundAccount;
            ControlParameters = controlParameters;
            Initiation = initiation;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets indicates whether information about RefundAccount
        /// should be included in the payment response.
        /// . Possible values include: 'Yes', 'No'
        /// </summary>
        [JsonProperty(PropertyName = "ReadRefundAccount")]
        public OBDomesticVRPConsentRequestDataReadRefundAccountEnum? ReadRefundAccount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ControlParameters")]
        public OBDomesticVRPControlParameters ControlParameters { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBDomesticVRPInitiation Initiation { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ControlParameters == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ControlParameters");
            }
            if (Initiation == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Initiation");
            }
            if (ControlParameters != null)
            {
                ControlParameters.Validate();
            }
            if (Initiation != null)
            {
                Initiation.Validate();
            }
        }
    }
}
