// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Pisp.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class OBWritePaymentDetailsResponse1Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWritePaymentDetailsResponse1Data class.
        /// </summary>
        public OBWritePaymentDetailsResponse1Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWritePaymentDetailsResponse1Data class.
        /// </summary>
        public OBWritePaymentDetailsResponse1Data(IList<OBWritePaymentDetailsResponse1DataPaymentStatusItem> paymentStatus = default(IList<OBWritePaymentDetailsResponse1DataPaymentStatusItem>))
        {
            PaymentStatus = paymentStatus;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PaymentStatus")]
        public IList<OBWritePaymentDetailsResponse1DataPaymentStatusItem> PaymentStatus { get; set; }

    }
}
