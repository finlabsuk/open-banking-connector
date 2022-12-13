// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Provides the details to identify the beneficiary account. </summary>
    public partial class OBCashAccount51
    {
        /// <summary> Initializes a new instance of OBCashAccount51. </summary>
        /// <param name="schemeName"> Name of the identification scheme, in a coded form as published in an external list. </param>
        /// <param name="identification"> Beneficiary account identification. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="schemeName"/> or <paramref name="identification"/> is null. </exception>
        public OBCashAccount51(string schemeName, string identification)
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

        /// <summary> Initializes a new instance of OBCashAccount51. </summary>
        /// <param name="schemeName"> Name of the identification scheme, in a coded form as published in an external list. </param>
        /// <param name="identification"> Beneficiary account identification. </param>
        /// <param name="name">
        /// The account name is the name or names of the account owner(s) represented at an account level, as displayed by the ASPSP&apos;s online channels.
        /// Note, the account name is not the product name or the nickname of the account.
        /// </param>
        /// <param name="secondaryIdentification">
        /// This is secondary identification of the account, as assigned by the account servicing institution. 
        /// This can be used by building societies to additionally identify accounts with a roll number (in addition to a sort code and account number combination).
        /// </param>
        [JsonConstructor]
        public OBCashAccount51(string schemeName, string identification, string name, string secondaryIdentification)
        {
            SchemeName = schemeName;
            Identification = identification;
            Name = name;
            SecondaryIdentification = secondaryIdentification;
        }

        /// <summary> Name of the identification scheme, in a coded form as published in an external list. </summary>
        public string SchemeName { get; }
        /// <summary> Beneficiary account identification. </summary>
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
