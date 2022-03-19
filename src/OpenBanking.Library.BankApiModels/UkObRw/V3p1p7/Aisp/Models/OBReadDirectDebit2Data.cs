// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> The OBReadDirectDebit2Data. </summary>
    public partial class OBReadDirectDebit2Data
    {
        /// <summary> Initializes a new instance of OBReadDirectDebit2Data. </summary>
        public OBReadDirectDebit2Data()
        {
            DirectDebit = new ChangeTrackingList<OBReadDirectDebit2DataDirectDebitItem>();
        }

        /// <summary> Initializes a new instance of OBReadDirectDebit2Data. </summary>
        /// <param name="directDebit"></param>
        public OBReadDirectDebit2Data(IReadOnlyList<OBReadDirectDebit2DataDirectDebitItem> directDebit)
        {
            DirectDebit = directDebit;
        }

        /// <summary> Gets the direct debit. </summary>
        public IReadOnlyList<OBReadDirectDebit2DataDirectDebitItem> DirectDebit { get; }
    }
}
