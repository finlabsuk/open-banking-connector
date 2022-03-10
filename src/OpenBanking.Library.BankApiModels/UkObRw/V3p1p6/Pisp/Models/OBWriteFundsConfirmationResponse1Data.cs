// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    [SourceApiEquivalent(typeof(V3p1p4.Pisp.Models.OBWriteFundsConfirmationResponse1Data))]
    public partial class OBWriteFundsConfirmationResponse1Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteFundsConfirmationResponse1Data class.
        /// </summary>
        public OBWriteFundsConfirmationResponse1Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteFundsConfirmationResponse1Data class.
        /// </summary>
        /// <param name="fundsAvailableResult">Result of a funds availability
        /// check.</param>
        public OBWriteFundsConfirmationResponse1Data(OBWriteFundsConfirmationResponse1DataFundsAvailableResult fundsAvailableResult = default(OBWriteFundsConfirmationResponse1DataFundsAvailableResult), IDictionary<string, object> supplementaryData = default(IDictionary<string, object>))
        {
            FundsAvailableResult = fundsAvailableResult;
            SupplementaryData = supplementaryData;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets result of a funds availability check.
        /// </summary>
        [JsonProperty(PropertyName = "FundsAvailableResult")]
        public OBWriteFundsConfirmationResponse1DataFundsAvailableResult FundsAvailableResult { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SupplementaryData")]
        public IDictionary<string, object> SupplementaryData { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (FundsAvailableResult != null)
            {
                FundsAvailableResult.Validate();
            }
        }
    }
}
