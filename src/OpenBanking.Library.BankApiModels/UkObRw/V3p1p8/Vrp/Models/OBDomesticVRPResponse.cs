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

    public partial class OBDomesticVRPResponse
    {
        /// <summary>
        /// Initializes a new instance of the OBDomesticVRPResponse class.
        /// </summary>
        public OBDomesticVRPResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBDomesticVRPResponse class.
        /// </summary>
        public OBDomesticVRPResponse(OBDomesticVRPResponseData data, OBRisk1 risk, Links links, object meta)
        {
            Data = data;
            Risk = risk;
            Links = links;
            Meta = meta;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Data")]
        public OBDomesticVRPResponseData Data { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Risk")]
        public OBRisk1 Risk { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Links")]
        public Links Links { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Meta")]
        public object Meta { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Data == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Data");
            }
            if (Risk == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Risk");
            }
            if (Links == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Links");
            }
            if (Meta == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Meta");
            }
            if (Data != null)
            {
                Data.Validate();
            }
            if (Risk != null)
            {
                Risk.Validate();
            }
            if (Links != null)
            {
                Links.Validate();
            }
        }
    }
}