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

    public partial class OBWriteFundsConfirmationResponse1
    {
        /// <summary>
        /// Initializes a new instance of the OBWriteFundsConfirmationResponse1
        /// class.
        /// </summary>
        public OBWriteFundsConfirmationResponse1()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBWriteFundsConfirmationResponse1
        /// class.
        /// </summary>
        public OBWriteFundsConfirmationResponse1(OBWriteFundsConfirmationResponse1Data data, Links links = default(Links), Meta meta = default(Meta))
        {
            Data = data;
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
        public OBWriteFundsConfirmationResponse1Data Data { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Links")]
        public Links Links { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Meta")]
        public Meta Meta { get; set; }

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
            if (Data != null)
            {
                Data.Validate();
            }
            if (Links != null)
            {
                Links.Validate();
            }
        }
    }
}
