using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation
{
    /// <summary>
    ///     Supporting Data provided by TPP, when requesting SCA Exemption.
    /// </summary>
    [OpenBankingEquivalent(typeof(OBWriteFileConsent3DataSCASupportData))]
    [PersistenceEquivalent(typeof(Persistent.PaymentInitiation.OBWriteDomesticConsentDataSCASupportData))]
    public class OBWriteDomesticConsentDataSCASupportData
    {
        

        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticConsentDataSCASupportData" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticConsentDataSCASupportData()
        {
        }

        /// <summary>
        ///     This field allows a PISP to request specific SCA Exemption for a Payment Initiation
        /// </summary>
        /// <value>This field allows a PISP to request specific SCA Exemption for a Payment Initiation</value>
        [JsonProperty("requestedScaExemptionType")]
        public RequestedSCAExemptionType? RequestedSCAExemptionType { get; set; }

        /// <summary>
        ///     Specifies a character string with a maximum length of 40 characters. Usage: This field indicates whether the PSU
        ///     was subject to SCA performed by the TPP
        /// </summary>
        /// <value>
        ///     Specifies a character string with a maximum length of 40 characters. Usage: This field indicates whether the PSU
        ///     was subject to SCA performed by the TPP
        /// </value>
        [JsonProperty("appliedAuthenticationApproach")]
        public AppliedAuthenticationApproach? AppliedAuthenticationApproach { get; set; }

        /// <summary>
        ///     Specifies a character string with a maximum length of 140 characters. Usage: If the payment is recurring then the
        ///     transaction identifier of the previous payment occurrence so that the ASPSP can verify that the PISP, amount and
        ///     the payee are the same as the previous occurrence.
        /// </summary>
        /// <value>
        ///     Specifies a character string with a maximum length of 140 characters. Usage: If the payment is recurring then
        ///     the transaction identifier of the previous payment occurrence so that the ASPSP can verify that the PISP, amount
        ///     and the payee are the same as the previous occurrence.
        /// </value>
        [JsonProperty("referencePaymentOrderId")]
        public string ReferencePaymentOrderId { get; set; }

    }
}