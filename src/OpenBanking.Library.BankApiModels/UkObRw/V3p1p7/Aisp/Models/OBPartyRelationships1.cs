// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> The Party&apos;s relationships with other resources. </summary>
    public partial class OBPartyRelationships1
    {
        /// <summary> Initializes a new instance of OBPartyRelationships1. </summary>
        public OBPartyRelationships1()
        {
        }

        /// <summary> Initializes a new instance of OBPartyRelationships1. </summary>
        /// <param name="account"> Relationship to the Account resource. </param>
        public OBPartyRelationships1(OBPartyRelationships1Account account)
        {
            Account = account;
        }

        /// <summary> Relationship to the Account resource. </summary>
        public OBPartyRelationships1Account Account { get; }
    }
}
