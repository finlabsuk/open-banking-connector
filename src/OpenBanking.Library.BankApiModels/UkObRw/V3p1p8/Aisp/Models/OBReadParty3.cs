// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> The OBReadParty3. </summary>
    public partial class OBReadParty3
    {
        /// <summary> Initializes a new instance of OBReadParty3. </summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
        internal OBReadParty3(OBReadParty3Data data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        /// <summary> Initializes a new instance of OBReadParty3. </summary>
        /// <param name="data"></param>
        /// <param name="links"> Links relevant to the payload. </param>
        /// <param name="meta"> Meta Data relevant to the payload. </param>
        internal OBReadParty3(OBReadParty3Data data, Links links, Meta meta)
        {
            Data = data;
            Links = links;
            Meta = meta;
        }

        /// <summary> Gets the data. </summary>
        public OBReadParty3Data Data { get; }
        /// <summary> Links relevant to the payload. </summary>
        public Links Links { get; }
        /// <summary> Meta Data relevant to the payload. </summary>
        public Meta Meta { get; }
    }
}
