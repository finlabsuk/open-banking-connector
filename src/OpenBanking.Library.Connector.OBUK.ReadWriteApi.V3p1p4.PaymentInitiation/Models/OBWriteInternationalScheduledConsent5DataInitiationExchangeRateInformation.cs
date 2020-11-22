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
    /// Provides details on the currency exchange rate and contract.
    /// </summary>
    public partial class OBWriteInternationalScheduledConsent5DataInitiationExchangeRateInformation
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsent5DataInitiationExchangeRateInformation
        /// class.
        /// </summary>
        public OBWriteInternationalScheduledConsent5DataInitiationExchangeRateInformation()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// OBWriteInternationalScheduledConsent5DataInitiationExchangeRateInformation
        /// class.
        /// </summary>
        /// <param name="unitCurrency">Currency in which the rate of exchange
        /// is expressed in a currency exchange. In the example 1GBP = xxxCUR,
        /// the unit currency is GBP.</param>
        /// <param name="rateType">Specifies the type used to complete the
        /// currency exchange. Possible values include: 'Actual', 'Agreed',
        /// 'Indicative'</param>
        /// <param name="exchangeRate">The factor used for conversion of an
        /// amount from one currency to another. This reflects the price at
        /// which one currency was bought with another currency.</param>
        /// <param name="contractIdentification">Unique and unambiguous
        /// reference to the foreign exchange contract agreed between the
        /// initiating party/creditor and the debtor agent.</param>
        public OBWriteInternationalScheduledConsent5DataInitiationExchangeRateInformation(string unitCurrency, OBWriteInternationalScheduledConsent5DataInitiationExchangeRateInformationRateTypeEnum rateType, double? exchangeRate = default(double?), string contractIdentification = default(string))
        {
            UnitCurrency = unitCurrency;
            ExchangeRate = exchangeRate;
            RateType = rateType;
            ContractIdentification = contractIdentification;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets currency in which the rate of exchange is expressed in
        /// a currency exchange. In the example 1GBP = xxxCUR, the unit
        /// currency is GBP.
        /// </summary>
        [JsonProperty(PropertyName = "UnitCurrency")]
        public string UnitCurrency { get; set; }

        /// <summary>
        /// Gets or sets the factor used for conversion of an amount from one
        /// currency to another. This reflects the price at which one currency
        /// was bought with another currency.
        /// </summary>
        [JsonProperty(PropertyName = "ExchangeRate")]
        public double? ExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets specifies the type used to complete the currency
        /// exchange. Possible values include: 'Actual', 'Agreed', 'Indicative'
        /// </summary>
        [JsonProperty(PropertyName = "RateType")]
        public OBWriteInternationalScheduledConsent5DataInitiationExchangeRateInformationRateTypeEnum RateType { get; set; }

        /// <summary>
        /// Gets or sets unique and unambiguous reference to the foreign
        /// exchange contract agreed between the initiating party/creditor and
        /// the debtor agent.
        /// </summary>
        [JsonProperty(PropertyName = "ContractIdentification")]
        public string ContractIdentification { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (UnitCurrency == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "UnitCurrency");
            }
        }
    }
}
