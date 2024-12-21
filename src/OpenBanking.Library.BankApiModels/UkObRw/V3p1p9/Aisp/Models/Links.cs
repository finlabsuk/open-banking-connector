// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Links relevant to the payload. </summary>
    [SourceApiEquivalent(typeof(V3p1p7.Aisp.Models.Links))]
    public partial class Links
    {
        /// <summary> Initializes a new instance of Links. </summary>
        /// <param name="self"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="self"/> is null. </exception>
        public Links(string self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            Self = self;
        }

        /// <summary> Initializes a new instance of Links. </summary>
        /// <param name="self"></param>
        /// <param name="first"></param>
        /// <param name="prev"></param>
        /// <param name="next"></param>
        /// <param name="last"></param>
        [JsonConstructor]
        public Links(string self, string first, string prev, string next, string last)
        {
            Self = self;
            First = first;
            Prev = prev;
            Next = next;
            Last = last;
        }

        /// <summary> Gets the self. </summary>
        public string Self { get; set; }

        /// <summary> Gets the first. </summary>
        public string First { get; set; }

        /// <summary> Gets the prev. </summary>
        public string Prev { get; set; }

        /// <summary> Gets the next. </summary>
        public string Next { get; set; }

        /// <summary> Gets the last. </summary>
        public string Last { get; set; }
    }
}