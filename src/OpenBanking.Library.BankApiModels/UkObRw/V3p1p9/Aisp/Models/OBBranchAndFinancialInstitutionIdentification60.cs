// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary>
    /// Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account.
    /// This is the servicer of the beneficiary account.
    /// </summary>
    public partial class OBBranchAndFinancialInstitutionIdentification60
    {
        /// <summary> Initializes a new instance of OBBranchAndFinancialInstitutionIdentification60. </summary>
        internal OBBranchAndFinancialInstitutionIdentification60()
        {
        }

        /// <summary> Initializes a new instance of OBBranchAndFinancialInstitutionIdentification60. </summary>
        /// <param name="schemeName"> Name of the identification scheme, in a coded form as published in an external list. </param>
        /// <param name="identification"> Unique and unambiguous identification of the servicing institution. </param>
        /// <param name="name"> Name by which an agent is known and which is usually used to identify that agent. </param>
        /// <param name="postalAddress"> Information that locates and identifies a specific address, as defined by postal services. </param>
        internal OBBranchAndFinancialInstitutionIdentification60(string schemeName, string identification, string name, OBPostalAddress6 postalAddress)
        {
            SchemeName = schemeName;
            Identification = identification;
            Name = name;
            PostalAddress = postalAddress;
        }

        /// <summary> Name of the identification scheme, in a coded form as published in an external list. </summary>
        public string SchemeName { get; }
        /// <summary> Unique and unambiguous identification of the servicing institution. </summary>
        public string Identification { get; }
        /// <summary> Name by which an agent is known and which is usually used to identify that agent. </summary>
        public string Name { get; }
        /// <summary> Information that locates and identifies a specific address, as defined by postal services. </summary>
        public OBPostalAddress6 PostalAddress { get; }
    }
}
