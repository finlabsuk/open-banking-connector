using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Amount of money to be moved between the debtor and creditor, before deduction of charges, expressed in the currency
    ///     as ordered by the initiating party. Usage: This amount has to be transported unchanged through the transaction
    ///     chain.
    /// </summary>
    public class OBWriteDomesticDataInitiationInstructedAmount
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticDataInitiationInstructedAmount" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticDataInitiationInstructedAmount()
        {
        }
        
        /// <summary>
        ///     A number of monetary units specified in an active currency where the unit of currency is explicit and compliant
        ///     with ISO 4217.
        /// </summary>
        /// <value>
        ///     A number of monetary units specified in an active currency where the unit of currency is explicit and compliant
        ///     with ISO 4217.
        /// </value>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        ///     A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described
        ///     in the latest edition of the international standard ISO 4217 \&quot;Codes for the representation of currencies and
        ///     funds\&quot;.
        /// </summary>
        /// <value>
        ///     A code allocated to a currency by a Maintenance Agency under an international identification scheme, as
        ///     described in the latest edition of the international standard ISO 4217 \&quot;Codes for the representation of
        ///     currencies and funds\&quot;.
        /// </value>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        
    }
}