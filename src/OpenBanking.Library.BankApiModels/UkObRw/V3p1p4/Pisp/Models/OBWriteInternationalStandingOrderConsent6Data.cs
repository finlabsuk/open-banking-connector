// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class OBWriteInternationalStandingOrderConsent6Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderConsent6Data class.
        /// </summary>
        public OBWriteInternationalStandingOrderConsent6Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalStandingOrderConsent6Data class.
        /// </summary>
        /// <param name="initiation">The Initiation payload is sent by the
        /// initiating party to the ASPSP. It is used to request movement of
        /// funds from the debtor account to a creditor for an international
        /// standing order.</param>
        /// <param name="readRefundAccount">Specifies to share the refund
        /// account details with PISP. Possible values include: 'No',
        /// 'Yes'</param>
        /// <param name="authorisation">The authorisation type request from the
        /// TPP.</param>
        /// <param name="sCASupportData">Supporting Data provided by TPP, when
        /// requesting SCA Exemption.</param>
        public OBWriteInternationalStandingOrderConsent6Data(OBWriteInternationalStandingOrderConsent6DataInitiation initiation, OBWriteInternationalStandingOrderConsent6DataReadRefundAccountEnum? readRefundAccount = default(OBWriteInternationalStandingOrderConsent6DataReadRefundAccountEnum?), OBWriteInternationalStandingOrderConsent6DataAuthorisation authorisation = default(OBWriteInternationalStandingOrderConsent6DataAuthorisation), OBWriteInternationalStandingOrderConsent6DataSCASupportData sCASupportData = default(OBWriteInternationalStandingOrderConsent6DataSCASupportData))
        {
            ReadRefundAccount = readRefundAccount;
            Initiation = initiation;
            Authorisation = authorisation;
            SCASupportData = sCASupportData;
            CustomInit();
        }
        /// <summary>
        /// Static constructor for
        /// OBWriteInternationalStandingOrderConsent6Data class.
        /// </summary>
        static OBWriteInternationalStandingOrderConsent6Data()
        {
            Permission = "Create";
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets specifies to share the refund account details with
        /// PISP. Possible values include: 'No', 'Yes'
        /// </summary>
        [JsonProperty(PropertyName = "ReadRefundAccount")]
        public OBWriteInternationalStandingOrderConsent6DataReadRefundAccountEnum? ReadRefundAccount { get; set; }

        /// <summary>
        /// Gets or sets the Initiation payload is sent by the initiating party
        /// to the ASPSP. It is used to request movement of funds from the
        /// debtor account to a creditor for an international standing order.
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBWriteInternationalStandingOrderConsent6DataInitiation Initiation { get; set; }

        /// <summary>
        /// Gets or sets the authorisation type request from the TPP.
        /// </summary>
        [JsonProperty(PropertyName = "Authorisation")]
        public OBWriteInternationalStandingOrderConsent6DataAuthorisation Authorisation { get; set; }

        /// <summary>
        /// Gets or sets supporting Data provided by TPP, when requesting SCA
        /// Exemption.
        /// </summary>
        [JsonProperty(PropertyName = "SCASupportData")]
        public OBWriteInternationalStandingOrderConsent6DataSCASupportData SCASupportData { get; set; }

        /// <summary>
        /// Specifies the Open Banking service request types.
        /// </summary>
        [JsonProperty(PropertyName = "Permission")]
        public static string Permission { get; private set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Initiation == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Initiation");
            }
            if (Initiation != null)
            {
                Initiation.Validate();
            }
            if (Authorisation != null)
            {
                Authorisation.Validate();
            }
        }
    }
}
