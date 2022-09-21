// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> The OBReadBeneficiary5. </summary>
    public partial class OBReadBeneficiary5
    {
        /// <summary> Initializes a new instance of OBReadBeneficiary5. </summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
        internal OBReadBeneficiary5(OBReadBeneficiary5Data data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        /// <summary> Initializes a new instance of OBReadBeneficiary5. </summary>
        /// <param name="data"></param>
        /// <param name="links"> Links relevant to the payload. </param>
        /// <param name="meta"> Meta Data relevant to the payload. </param>
        internal OBReadBeneficiary5(OBReadBeneficiary5Data data, Links links, Meta meta)
        {
            Data = data;
            Links = links;
            Meta = meta;
        }

        /// <summary> Gets the data. </summary>
        public OBReadBeneficiary5Data Data { get; }
        /// <summary> Links relevant to the payload. </summary>
        public Links Links { get; }
        /// <summary> Meta Data relevant to the payload. </summary>
        public Meta Meta { get; }
    }
}
