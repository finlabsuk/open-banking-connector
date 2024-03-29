// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary>
    ///     Response object OBReadBalance1 from UK Open Banking Read-Write Account and Transaction API spec
    ///     v3.1.11. Open Banking Connector will automatically
    ///     translate <i>to</i> this from an older format for banks supporting an earlier spec version.
    /// </summary>
    [SourceApiEquivalent(typeof(V3p1p7.Aisp.Models.OBReadBalance1))]
    public partial class OBReadBalance1
    {
        /// <summary> Initializes a new instance of OBReadBalance1. </summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="data"/> is null. </exception>
        public OBReadBalance1(OBReadBalance1Data data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Data = data;
        }

        /// <summary> Initializes a new instance of OBReadBalance1. </summary>
        /// <param name="data"></param>
        /// <param name="links"> Links relevant to the payload. </param>
        /// <param name="meta"> Meta Data relevant to the payload. </param>
        [JsonConstructor]
        public OBReadBalance1(OBReadBalance1Data data, Links links, Meta meta)
        {
            Data = data;
            Links = links;
            Meta = meta;
        }

        /// <summary> Gets the data. </summary>
        public OBReadBalance1Data Data { get; }
        /// <summary> Links relevant to the payload. </summary>
        public Links Links { get; }
        /// <summary> Meta Data relevant to the payload. </summary>
        public Meta Meta { get; }
    }
}
