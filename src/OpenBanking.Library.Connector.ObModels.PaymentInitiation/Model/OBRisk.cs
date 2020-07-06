// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    /// <summary>
    ///     The Risk section is sent by the initiating party to the ASPSP. It is used to specify additional details for risk
    ///     scoring for Payments.
    /// </summary>
    [OpenBankingEquivalent(typeof(OBRisk1))]
    [OpenBankingEquivalent(typeof(ObModels.PaymentInitiation.V3p1p2.Model.OBRisk1))]
    public class OBRisk
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBRisk" /> class.
        /// </summary>
        [JsonConstructor]
        public OBRisk() { }

        /// <summary>
        ///     Specifies the payment context
        /// </summary>
        /// <value>Specifies the payment context</value>
        [JsonProperty("paymentContextCode")]
        public PaymentContextCode? PaymentContextCode { get; set; }

        /// <summary>
        ///     Category code conform to ISO 18245, related to the type of services or goods the merchant provides for the
        ///     transaction.
        /// </summary>
        /// <value>
        ///     Category code conform to ISO 18245, related to the type of services or goods the merchant provides for the
        ///     transaction.
        /// </value>
        [JsonProperty("merchantCategoryCode")]
        public string MerchantCategoryCode { get; set; }

        /// <summary>
        ///     The unique customer identifier of the PSU with the merchant.
        /// </summary>
        /// <value>The unique customer identifier of the PSU with the merchant.</value>
        [JsonProperty("merchantCustomerIdentification")]
        public string MerchantCustomerIdentification { get; set; }

        /// <summary>
        ///     Gets or Sets DeliveryAddress
        /// </summary>
        [JsonProperty("deliveryAddress")]
        public OBRiskDeliveryAddress DeliveryAddress { get; set; }
    }
}
