// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Provides the details to identify an account. </summary>
    internal partial class OBAccount4DetailAccountItem
    {
        /// <summary> Initializes a new instance of OBAccount4DetailAccountItem. </summary>
        /// <param name="schemeName"> Name of the identification scheme, in a coded form as published in an external list. </param>
        /// <param name="identification"> Identification assigned by an institution to identify an account. This identification is known by the account owner. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="schemeName"/> or <paramref name="identification"/> is null. </exception>
        internal OBAccount4DetailAccountItem(string schemeName, string identification)
        {
            if (schemeName == null)
            {
                throw new ArgumentNullException(nameof(schemeName));
            }
            if (identification == null)
            {
                throw new ArgumentNullException(nameof(identification));
            }

            SchemeName = schemeName;
            Identification = identification;
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
