// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> The OBReadParty3Data. </summary>
    public partial class OBReadParty3Data
    {
        /// <summary> Initializes a new instance of OBReadParty3Data. </summary>
        internal OBReadParty3Data()
        {
            Party = new ChangeTrackingList<OBParty2>();
        }

        /// <summary> Initializes a new instance of OBReadParty3Data. </summary>
        /// <param name="party"></param>
        internal OBReadParty3Data(IReadOnlyList<OBParty2> party)
        {
            Party = party;
        }

        /// <summary> Gets the party. </summary>
        public IReadOnlyList<OBParty2> Party { get; }
    }
}
