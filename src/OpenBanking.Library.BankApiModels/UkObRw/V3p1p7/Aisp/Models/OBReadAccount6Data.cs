// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> The OBReadAccount6Data. </summary>
    public partial class OBReadAccount6Data
    {
        /// <summary> Initializes a new instance of OBReadAccount6Data. </summary>
        public OBReadAccount6Data()
        {
            Account = new ChangeTrackingList<OBAccount6>();
        }

        /// <summary> Initializes a new instance of OBReadAccount6Data. </summary>
        /// <param name="account"></param>
        public OBReadAccount6Data(IReadOnlyList<OBAccount6> account)
        {
            Account = account;
        }

        /// <summary> Gets the account. </summary>
        public IReadOnlyList<OBAccount6> Account { get; }
    }
}