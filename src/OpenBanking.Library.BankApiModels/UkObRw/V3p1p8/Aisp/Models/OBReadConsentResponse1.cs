// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> The OBReadConsentResponse1. </summary>
    public partial class OBReadConsentResponse1
    {
        /// <summary> Initializes a new instance of OBReadConsentResponse1. </summary>
        /// <param name="data"></param>
        /// <param name="risk"> Any object. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="data"/> or <paramref name="risk"/> is null. </exception>
        public OBReadConsentResponse1(OBReadConsentResponse1Data data, object risk)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (risk == null)
            {
                throw new ArgumentNullException(nameof(risk));
            }

            Data = data;
            Risk = risk;
        }

        /// <summary> Initializes a new instance of OBReadConsentResponse1. </summary>
        /// <param name="data"></param>
        /// <param name="risk"> Any object. </param>
        /// <param name="links"> Links relevant to the payload. </param>
        /// <param name="meta"> Meta Data relevant to the payload. </param>
        public OBReadConsentResponse1(OBReadConsentResponse1Data data, object risk, Links links, Meta meta)
        {
            Data = data;
            Risk = risk;
            Links = links;
            Meta = meta;
        }

        /// <summary> Gets the data. </summary>
        public OBReadConsentResponse1Data Data { get; }
        /// <summary> Any object. </summary>
        public object Risk { get; }
        /// <summary> Links relevant to the payload. </summary>
        public Links Links { get; }
        /// <summary> Meta Data relevant to the payload. </summary>
        public Meta Meta { get; }
    }
}
