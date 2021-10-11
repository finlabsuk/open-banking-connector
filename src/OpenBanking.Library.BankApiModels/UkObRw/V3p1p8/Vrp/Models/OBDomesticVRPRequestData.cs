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

    [SourceApiEquivalent(typeof(OBDomesticVRPConsentRequestData))]

    public partial class OBDomesticVRPRequestData
    {
        /// <summary>
        /// Initializes a new instance of the OBDomesticVRPRequestData class.
        /// </summary>
        public OBDomesticVRPRequestData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBDomesticVRPRequestData class.
        /// </summary>
        /// <param name="consentId">Identifier for the Domestic VRP Consent
        /// that this payment is made under.</param>
        /// <param name="pSUAuthenticationMethod">^ The authentication method
        /// that was used to authenicate the PSU.   |
        /// OBVRPAuthenticationMethods - Namespaced Enumeration</param>
        public OBDomesticVRPRequestData(string consentId, string pSUAuthenticationMethod, OBDomesticVRPInitiation initiation, OBDomesticVRPInstruction instruction)
        {
            ConsentId = consentId;
            PSUAuthenticationMethod = pSUAuthenticationMethod;
            Initiation = initiation;
            Instruction = instruction;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets identifier for the Domestic VRP Consent that this
        /// payment is made under.
        /// </summary>
        [JsonProperty(PropertyName = "ConsentId")]
        public string ConsentId { get; set; }

        /// <summary>
        /// Gets or sets ^ The authentication method that was used to
        /// authenicate the PSU.   | OBVRPAuthenticationMethods - Namespaced
        /// Enumeration
        /// </summary>
        [JsonProperty(PropertyName = "PSUAuthenticationMethod")]
        public string PSUAuthenticationMethod { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBDomesticVRPInitiation Initiation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Instruction")]
        public OBDomesticVRPInstruction Instruction { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ConsentId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ConsentId");
            }
            if (PSUAuthenticationMethod == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "PSUAuthenticationMethod");
            }
            if (Initiation == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Initiation");
            }
            if (Instruction == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Instruction");
            }
            if (ConsentId != null)
            {
                if (ConsentId.Length > 128)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "ConsentId", 128);
                }
            }
            if (Initiation != null)
            {
                Initiation.Validate();
            }
            if (Instruction != null)
            {
                Instruction.Validate();
            }
        }
    }
}
