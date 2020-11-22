// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.Connector.OBUK.ReadWriteApi.V3p1p2.PaymentInitiation.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Supporting Data provided by TPP, when requesting SCA Exemption.
    /// </summary>
    public partial class OBWriteDomesticStandingOrderConsent4DataSCASupportData
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticStandingOrderConsent4DataSCASupportData class.
        /// </summary>
        public OBWriteDomesticStandingOrderConsent4DataSCASupportData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticStandingOrderConsent4DataSCASupportData class.
        /// </summary>
        /// <param name="requestedSCAExemptionType">This field allows a PISP to
        /// request specific SCA Exemption for a Payment Initiation. Possible
        /// values include: 'BillPayment', 'ContactlessTravel',
        /// 'EcommerceGoods', 'EcommerceServices', 'Kiosk', 'Parking',
        /// 'PartyToParty'</param>
        /// <param name="appliedAuthenticationApproach">Specifies a character
        /// string with a maximum length of 40 characters.
        /// Usage: This field indicates whether the PSU was subject to SCA
        /// performed by the TPP. Possible values include: 'CA', 'SCA'</param>
        /// <param name="referencePaymentOrderId">Specifies a character string
        /// with a maximum length of 140 characters.
        /// Usage: If the payment is recurring then the transaction identifier
        /// of the previous payment occurrence so that the ASPSP can verify
        /// that the PISP, amount and the payee are the same as the previous
        /// occurrence.</param>
        public OBWriteDomesticStandingOrderConsent4DataSCASupportData(OBWriteDomesticStandingOrderConsent4DataSCASupportDataRequestedSCAExemptionTypeEnum? requestedSCAExemptionType = default(OBWriteDomesticStandingOrderConsent4DataSCASupportDataRequestedSCAExemptionTypeEnum?), OBWriteDomesticStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum? appliedAuthenticationApproach = default(OBWriteDomesticStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum?), string referencePaymentOrderId = default(string))
        {
            RequestedSCAExemptionType = requestedSCAExemptionType;
            AppliedAuthenticationApproach = appliedAuthenticationApproach;
            ReferencePaymentOrderId = referencePaymentOrderId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets this field allows a PISP to request specific SCA
        /// Exemption for a Payment Initiation. Possible values include:
        /// 'BillPayment', 'ContactlessTravel', 'EcommerceGoods',
        /// 'EcommerceServices', 'Kiosk', 'Parking', 'PartyToParty'
        /// </summary>
        [JsonProperty(PropertyName = "RequestedSCAExemptionType")]
        public OBWriteDomesticStandingOrderConsent4DataSCASupportDataRequestedSCAExemptionTypeEnum? RequestedSCAExemptionType { get; set; }

        /// <summary>
        /// Gets or sets specifies a character string with a maximum length of
        /// 40 characters.
        /// Usage: This field indicates whether the PSU was subject to SCA
        /// performed by the TPP. Possible values include: 'CA', 'SCA'
        /// </summary>
        [JsonProperty(PropertyName = "AppliedAuthenticationApproach")]
        public OBWriteDomesticStandingOrderConsent4DataSCASupportDataAppliedAuthenticationApproachEnum? AppliedAuthenticationApproach { get; set; }

        /// <summary>
        /// Gets or sets specifies a character string with a maximum length of
        /// 140 characters.
        /// Usage: If the payment is recurring then the transaction identifier
        /// of the previous payment occurrence so that the ASPSP can verify
        /// that the PISP, amount and the payee are the same as the previous
        /// occurrence.
        /// </summary>
        [JsonProperty(PropertyName = "ReferencePaymentOrderId")]
        public string ReferencePaymentOrderId { get; set; }

    }
}
