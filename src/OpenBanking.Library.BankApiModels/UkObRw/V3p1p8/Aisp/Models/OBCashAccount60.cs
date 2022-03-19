// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> Unambiguous identification of the account of the creditor, in the case of a debit transaction. </summary>
    public partial class OBCashAccount60
    {
        /// <summary> Initializes a new instance of OBCashAccount60. </summary>
        public OBCashAccount60()
        {
        }

        /// <summary> Initializes a new instance of OBCashAccount60. </summary>
        /// <param name="schemeName"> Name of the identification scheme, in a coded form as published in an external list. </param>
        /// <param name="identification"> Identification assigned by an institution to identify an account. This identification is known by the account owner. </param>
        /// <param name="name">
        /// The account name is the name or names of the account owner(s) represented at an account level, as displayed by the ASPSP&apos;s online channels.
        /// Note, the account name is not the product name or the nickname of the account.
        /// </param>
        /// <param name="secondaryIdentification">
        /// This is secondary identification of the account, as assigned by the account servicing institution. 
        /// This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).
        /// </param>
        public OBCashAccount60(string schemeName, string identification, string name, string secondaryIdentification)
        {
            SchemeName = schemeName;
            Identification = identification;
            Name = name;
            SecondaryIdentification = secondaryIdentification;
        }

        /// <summary> Name of the identification scheme, in a coded form as published in an external list. </summary>
        public string SchemeName { get; }
        /// <summary> Identification assigned by an institution to identify an account. This identification is known by the account owner. </summary>
        public string Identification { get; }
        /// <summary>
        /// The account name is the name or names of the account owner(s) represented at an account level, as displayed by the ASPSP&apos;s online channels.
        /// Note, the account name is not the product name or the nickname of the account.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// This is secondary identification of the account, as assigned by the account servicing institution. 
        /// This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).
        /// </summary>
        public string SecondaryIdentification { get; }
    }
}
