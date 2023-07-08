// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> The OBReadStatement2. </summary>
    public partial class OBReadStatement2
    {
        /// <summary> Initializes a new instance of OBReadStatement2. </summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
        public OBReadStatement2(OBReadDataStatement2 data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        /// <summary> Initializes a new instance of OBReadStatement2. </summary>
        /// <param name="data"></param>
        /// <param name="links"> Links relevant to the payload. </param>
        /// <param name="meta"> Meta Data relevant to the payload. </param>
        [JsonConstructor]
        public OBReadStatement2(OBReadDataStatement2 data, Links links, Meta meta)
        {
            Data = data;
            Links = links;
            Meta = meta;
        }

        /// <summary> Gets the data. </summary>
        public OBReadDataStatement2 Data { get; }
        /// <summary> Links relevant to the payload. </summary>
        public Links Links { get; }
        /// <summary> Meta Data relevant to the payload. </summary>
        public Meta Meta { get; }
    }
}
