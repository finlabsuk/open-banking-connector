// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> The OBReadStandingOrder6Data. </summary>
    public partial class OBReadStandingOrder6Data
    {
        /// <summary> Initializes a new instance of OBReadStandingOrder6Data. </summary>
        internal OBReadStandingOrder6Data()
        {
            StandingOrder = new ChangeTrackingList<OBStandingOrder6>();
        }

        /// <summary> Initializes a new instance of OBReadStandingOrder6Data. </summary>
        /// <param name="standingOrder"></param>
        internal OBReadStandingOrder6Data(IReadOnlyList<OBStandingOrder6> standingOrder)
        {
            StandingOrder = standingOrder;
        }

        /// <summary> Gets the standing order. </summary>
        public IReadOnlyList<OBStandingOrder6> StandingOrder { get; }
    }
}
