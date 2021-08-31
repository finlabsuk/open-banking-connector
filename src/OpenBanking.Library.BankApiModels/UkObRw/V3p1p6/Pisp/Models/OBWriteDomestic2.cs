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

    [TargetApiEquivalent(typeof(V3p1p4.Pisp.Models.OBWriteDomestic2))]
    public partial class OBWriteDomestic2
    {
        /// <summary>
        /// Initializes a new instance of the OBWriteDomestic2 class.
        /// </summary>
        public OBWriteDomestic2()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBWriteDomestic2 class.
        /// </summary>
        public OBWriteDomestic2(OBWriteDomestic2Data data, OBRisk1 risk)
        {
            Data = data;
            Risk = risk;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Data")]
        public OBWriteDomestic2Data Data { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Risk")]
        public OBRisk1 Risk { get; set; }

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
            if (Data != null)
            {
                Data.Validate();
            }
            if (Risk != null)
            {
                Risk.Validate();
            }
        }
    }
}
