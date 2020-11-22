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
    /// Payment status details as per underlying Payment Rail.
    /// </summary>
    public partial class OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetail
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetail
        /// class.
        /// </summary>
        public OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetail()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetail
        /// class.
        /// </summary>
        /// <param name="status">Status of a transfer, as assigned by the
        /// transaction administrator.</param>
        /// <param name="statusReason">Reason Code provided for the status of a
        /// transfer. Possible values include: 'Cancelled',
        /// 'PendingFailingSettlement', 'PendingSettlement', 'Proprietary',
        /// 'ProprietaryRejection', 'Suspended', 'Unmatched'</param>
        /// <param name="statusReasonDescription">Reason provided for the
        /// status of a transfer.</param>
        public OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetail(string status, string localInstrument = default(string), OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetailStatusReasonEnum? statusReason = default(OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetailStatusReasonEnum?), string statusReasonDescription = default(string))
        {
            LocalInstrument = localInstrument;
            Status = status;
            StatusReason = statusReason;
            StatusReasonDescription = statusReasonDescription;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LocalInstrument")]
        public string LocalInstrument { get; set; }

        /// <summary>
        /// Gets or sets status of a transfer, as assigned by the transaction
        /// administrator.
        /// </summary>
        [JsonProperty(PropertyName = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets reason Code provided for the status of a transfer.
        /// Possible values include: 'Cancelled', 'PendingFailingSettlement',
        /// 'PendingSettlement', 'Proprietary', 'ProprietaryRejection',
        /// 'Suspended', 'Unmatched'
        /// </summary>
        [JsonProperty(PropertyName = "StatusReason")]
        public OBWritePaymentDetailsResponse1DataPaymentStatusItemStatusDetailStatusReasonEnum? StatusReason { get; set; }

        /// <summary>
        /// Gets or sets reason provided for the status of a transfer.
        /// </summary>
        [JsonProperty(PropertyName = "StatusReasonDescription")]
        public string StatusReasonDescription { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Status == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Status");
            }
        }
    }
}
