using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    public class OBCashAccountDebtor
    {
        [JsonProperty("SchemeName")]
        public string SchemeName { get; set; }

        /// <summary>
        /// Identification assigned by an institution to identify an account. This identification is known by the account owner.
        /// </summary>
        /// <value>Identification assigned by an institution to identify an account. This identification is known by the account owner.</value>
        [JsonProperty("Identification")]
        public string Identification { get; set; }

        /// <summary>
        /// Name of the account, as assigned by the account servicing institution. Usage: The account name is the name or names of the account owner(s) represented at an account level. The account name is not the product name or the nickname of the account.
        /// </summary>
        /// <value>Name of the account, as assigned by the account servicing institution. Usage: The account name is the name or names of the account owner(s) represented at an account level. The account name is not the product name or the nickname of the account.</value>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// This is secondary identification of the account, as assigned by the account servicing institution.  This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).
        /// </summary>
        /// <value>This is secondary identification of the account, as assigned by the account servicing institution.  This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).</value>
        [JsonProperty("SecondaryIdentification")]
        public string SecondaryIdentification { get; set; }
    }
}
