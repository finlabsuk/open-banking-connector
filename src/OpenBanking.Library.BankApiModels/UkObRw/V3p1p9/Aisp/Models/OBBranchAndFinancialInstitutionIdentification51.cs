// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary>
    /// Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account.
    /// This is the servicer of the beneficiary account.
    /// </summary>
    public partial class OBBranchAndFinancialInstitutionIdentification51
    {
        /// <summary> Initializes a new instance of OBBranchAndFinancialInstitutionIdentification51. </summary>
        /// <param name="schemeName"> Name of the identification scheme, in a coded form as published in an external list. </param>
        /// <param name="identification"> Unique and unambiguous identification of the servicing institution. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="schemeName"/> or <paramref name="identification"/> is null. </exception>
        internal OBBranchAndFinancialInstitutionIdentification51(string schemeName, string identification)
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
        /// <summary> Unique and unambiguous identification of the servicing institution. </summary>
        public string Identification { get; }
    }
}
