// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> Financial institution servicing an account for the debtor. </summary>
    [SourceApiEquivalent(typeof(V3p1p7.Aisp.Models.OBBranchAndFinancialInstitutionIdentification62))]
    public partial class OBBranchAndFinancialInstitutionIdentification62
    {
        /// <summary> Initializes a new instance of OBBranchAndFinancialInstitutionIdentification62. </summary>
        public OBBranchAndFinancialInstitutionIdentification62()
        {
        }

        /// <summary> Initializes a new instance of OBBranchAndFinancialInstitutionIdentification62. </summary>
        /// <param name="schemeName"> Name of the identification scheme, in a coded form as published in an external list. </param>
        /// <param name="identification"> Unique and unambiguous identification of a financial institution or a branch of a financial institution. </param>
        /// <param name="name"> Name by which an agent is known and which is usually used to identify that agent. </param>
        /// <param name="postalAddress"> Information that locates and identifies a specific address, as defined by postal services. </param>
        public OBBranchAndFinancialInstitutionIdentification62(string schemeName, string identification, string name, OBPostalAddress6 postalAddress)
        {
            SchemeName = schemeName;
            Identification = identification;
            Name = name;
            PostalAddress = postalAddress;
        }

        /// <summary> Name of the identification scheme, in a coded form as published in an external list. </summary>
        public string SchemeName { get; }
        /// <summary> Unique and unambiguous identification of a financial institution or a branch of a financial institution. </summary>
        public string Identification { get; }
        /// <summary> Name by which an agent is known and which is usually used to identify that agent. </summary>
        public string Name { get; }
        /// <summary> Information that locates and identifies a specific address, as defined by postal services. </summary>
        public OBPostalAddress6 PostalAddress { get; }
    }
}
