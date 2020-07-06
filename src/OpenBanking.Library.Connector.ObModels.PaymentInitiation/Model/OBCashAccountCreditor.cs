// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    [SourceApiEquivalent(typeof(OBCashAccountCreditor3))]
    public class OBCashAccountCreditor
    {
        [JsonProperty("schemeName")]
        public string SchemeName { get; set; }

        /// <summary>
        ///     Identification assigned by an institution to identify an account. This identification is known by the account
        ///     owner.
        /// </summary>
        /// <value>
        ///     Identification assigned by an institution to identify an account. This identification is known by the account
        ///     owner.
        /// </value>
        [JsonProperty("identification")]
        public string Identification { get; set; }

        /// <summary>
        ///     Name of the account, as assigned by the account servicing institution. Usage: The account name is the name or names
        ///     of the account owner(s) represented at an account level. The account name is not the product name or the nickname
        ///     of the account. OB: ASPSPs may carry out name validation for Confirmation of Payee, but it is not mandatory.
        /// </summary>
        /// <value>
        ///     Name of the account, as assigned by the account servicing institution. Usage: The account name is the name or
        ///     names of the account owner(s) represented at an account level. The account name is not the product name or the
        ///     nickname of the account. OB: ASPSPs may carry out name validation for Confirmation of Payee, but it is not
        ///     mandatory.
        /// </value>
        [JsonProperty("name")]
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
        [JsonProperty("secondaryIdentification")]
        public string SecondaryIdentification { get; set; }
    }
}
