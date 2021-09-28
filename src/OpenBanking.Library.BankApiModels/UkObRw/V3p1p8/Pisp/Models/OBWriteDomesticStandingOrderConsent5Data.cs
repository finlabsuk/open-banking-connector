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

    public partial class OBWriteDomesticStandingOrderConsent5Data
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticStandingOrderConsent5Data class.
        /// </summary>
        public OBWriteDomesticStandingOrderConsent5Data()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteDomesticStandingOrderConsent5Data class.
        /// </summary>
        /// <param name="initiation">The Initiation payload is sent by the
        /// initiating party to the ASPSP. It is used to request movement of
        /// funds from the debtor account to a creditor for a domestic standing
        /// order.</param>
        /// <param name="readRefundAccount">Specifies to share the refund
        /// account details with PISP. Possible values include: 'No',
        /// 'Yes'</param>
        /// <param name="authorisation">The authorisation type request from the
        /// TPP.</param>
        public OBWriteDomesticStandingOrderConsent5Data(OBWriteDomesticStandingOrderConsent5DataInitiation initiation, OBWriteDomesticStandingOrderConsent5DataReadRefundAccountEnum? readRefundAccount = default(OBWriteDomesticStandingOrderConsent5DataReadRefundAccountEnum?), OBWriteDomesticStandingOrderConsent5DataAuthorisation authorisation = default(OBWriteDomesticStandingOrderConsent5DataAuthorisation), OBSCASupportData1 sCASupportData = default(OBSCASupportData1))
        {
            ReadRefundAccount = readRefundAccount;
            Initiation = initiation;
            Authorisation = authorisation;
            SCASupportData = sCASupportData;
            CustomInit();
        }
        /// <summary>
        /// Static constructor for OBWriteDomesticStandingOrderConsent5Data
        /// class.
        /// </summary>
        static OBWriteDomesticStandingOrderConsent5Data()
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
        public OBWriteDomesticStandingOrderConsent5DataReadRefundAccountEnum? ReadRefundAccount { get; set; }

        /// <summary>
        /// Gets or sets the Initiation payload is sent by the initiating party
        /// to the ASPSP. It is used to request movement of funds from the
        /// debtor account to a creditor for a domestic standing order.
        /// </summary>
        [JsonProperty(PropertyName = "Initiation")]
        public OBWriteDomesticStandingOrderConsent5DataInitiation Initiation { get; set; }

        /// <summary>
        /// Gets or sets the authorisation type request from the TPP.
        /// </summary>
        [JsonProperty(PropertyName = "Authorisation")]
        public OBWriteDomesticStandingOrderConsent5DataAuthorisation Authorisation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SCASupportData")]
        public OBSCASupportData1 SCASupportData { get; set; }

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
            if (SCASupportData != null)
            {
                SCASupportData.Validate();
            }
        }
    }
}
