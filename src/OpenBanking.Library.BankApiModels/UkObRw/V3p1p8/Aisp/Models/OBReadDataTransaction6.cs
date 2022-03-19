// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> The OBReadDataTransaction6. </summary>
    public partial class OBReadDataTransaction6
    {
        /// <summary> Initializes a new instance of OBReadDataTransaction6. </summary>
        public OBReadDataTransaction6()
        {
            Transaction = new ChangeTrackingList<OBTransaction6>();
        }

        /// <summary> Initializes a new instance of OBReadDataTransaction6. </summary>
        /// <param name="transaction"></param>
        public OBReadDataTransaction6(IReadOnlyList<OBTransaction6> transaction)
        {
            Transaction = transaction;
        }

        /// <summary> Gets the transaction. </summary>
        public IReadOnlyList<OBTransaction6> Transaction { get; }
    }
}
