using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Unambiguous identification of the account of the creditor to which a credit entry will be posted as a result of the
    ///     payment transaction.
    /// </summary>
    public class OBWriteDomesticDataInitiationCreditorAccount
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OBWriteDomesticDataInitiationCreditorAccount" /> class.
        /// </summary>
        [JsonConstructor]
        public OBWriteDomesticDataInitiationCreditorAccount()
        {
        }


        /// <summary>
        ///     Name of the identification scheme, in a coded form as published in an external list.
        /// </summary>
        /// <value>Name of the identification scheme, in a coded form as published in an external list.</value>
        [JsonProperty("SchemeName")]
        public string SchemeName { get; set; }

        /// <summary>
        ///     Identification assigned by an institution to identify an account. This identification is known by the account
        ///     owner.
        /// </summary>
        /// <value>
        ///     Identification assigned by an institution to identify an account. This identification is known by the account
        ///     owner.
        /// </value>
        [JsonProperty("Identification")]
        public string Identification { get; set; }

        /// <summary>
        ///     The account name is the name or names of the account owner(s) represented at an account level. Note, the account
        ///     name is not the product name or the nickname of the account. OB: ASPSPs may carry out name validation for
        ///     Confirmation of Payee, but it is not mandatory.
        /// </summary>
        /// <value>
        ///     The account name is the name or names of the account owner(s) represented at an account level. Note, the account
        ///     name is not the product name or the nickname of the account. OB: ASPSPs may carry out name validation for
        ///     Confirmation of Payee, but it is not mandatory.
        /// </value>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        ///     This is secondary identification of the account, as assigned by the account servicing institution.  This can be
        ///     used by building societies to additionally identify accounts with a roll number (in addition to a sort code and
        ///     account number combination).
        /// </summary>
        /// <value>
        ///     This is secondary identification of the account, as assigned by the account servicing institution.  This can be
        ///     used by building societies to additionally identify accounts with a roll number (in addition to a sort code and
        ///     account number combination).
        /// </value>
        [JsonProperty("SecondaryIdentification")]
        public string SecondaryIdentification { get; set; }

        
    }
}