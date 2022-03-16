// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> The OBReadParty2Data. </summary>
    public partial class OBReadParty2Data
    {
        /// <summary> Initializes a new instance of OBReadParty2Data. </summary>
        internal OBReadParty2Data()
        {
        }

        /// <summary> Initializes a new instance of OBReadParty2Data. </summary>
        /// <param name="party"></param>
        internal OBReadParty2Data(OBParty2 party)
        {
            Party = party;
        }

        /// <summary> Gets the party. </summary>
        public OBParty2 Party { get; }
    }
}
