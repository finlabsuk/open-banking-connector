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

    /// <summary>
    /// The Risk section is sent by the initiating party to the ASPSP. It is
    /// used to specify additional details for risk scoring for Payments.
    /// </summary>
    public partial class OBRisk1
    {
        /// <summary>
        /// Initializes a new instance of the OBRisk1 class.
        /// </summary>
        public OBRisk1()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OBRisk1 class.
        /// </summary>
        /// <param name="paymentContextCode">Specifies the payment context.
        /// Possible values include: 'BillPayment', 'EcommerceGoods',
        /// 'EcommerceServices', 'Other', 'PartyToParty'</param>
        /// <param name="merchantCategoryCode">Category code conform to ISO
        /// 18245, related to the type of services or goods the merchant
        /// provides for the transaction.</param>
        /// <param name="merchantCustomerIdentification">The unique customer
        /// identifier of the PSU with the merchant.</param>
        /// <param name="deliveryAddress">Information that locates and
        /// identifies a specific address, as defined by postal services or in
        /// free format text.</param>
        public OBRisk1(OBRisk1PaymentContextCodeEnum? paymentContextCode = default(OBRisk1PaymentContextCodeEnum?), string merchantCategoryCode = default(string), string merchantCustomerIdentification = default(string), OBRisk1DeliveryAddress deliveryAddress = default(OBRisk1DeliveryAddress))
        {
            PaymentContextCode = paymentContextCode;
            MerchantCategoryCode = merchantCategoryCode;
            MerchantCustomerIdentification = merchantCustomerIdentification;
            DeliveryAddress = deliveryAddress;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets specifies the payment context. Possible values
        /// include: 'BillPayment', 'EcommerceGoods', 'EcommerceServices',
        /// 'Other', 'PartyToParty'
        /// </summary>
        [JsonProperty(PropertyName = "PaymentContextCode")]
        public OBRisk1PaymentContextCodeEnum? PaymentContextCode { get; set; }

        /// <summary>
        /// Gets or sets category code conform to ISO 18245, related to the
        /// type of services or goods the merchant provides for the
        /// transaction.
        /// </summary>
        [JsonProperty(PropertyName = "MerchantCategoryCode")]
        public string MerchantCategoryCode { get; set; }

        /// <summary>
        /// Gets or sets the unique customer identifier of the PSU with the
        /// merchant.
        /// </summary>
        [JsonProperty(PropertyName = "MerchantCustomerIdentification")]
        public string MerchantCustomerIdentification { get; set; }

        /// <summary>
        /// Gets or sets information that locates and identifies a specific
        /// address, as defined by postal services or in free format text.
        /// </summary>
        [JsonProperty(PropertyName = "DeliveryAddress")]
        public OBRisk1DeliveryAddress DeliveryAddress { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (MerchantCategoryCode != null)
            {
                if (MerchantCategoryCode.Length > 4)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "MerchantCategoryCode", 4);
                }
                if (MerchantCategoryCode.Length < 3)
                {
                    throw new ValidationException(ValidationRules.MinLength, "MerchantCategoryCode", 3);
                }
            }
            if (MerchantCustomerIdentification != null)
            {
                if (MerchantCustomerIdentification.Length > 70)
                {
                    throw new ValidationException(ValidationRules.MaxLength, "MerchantCustomerIdentification", 70);
                }
                if (MerchantCustomerIdentification.Length < 1)
                {
                    throw new ValidationException(ValidationRules.MinLength, "MerchantCustomerIdentification", 1);
                }
            }
            if (DeliveryAddress != null)
            {
                DeliveryAddress.Validate();
            }
        }
    }
}