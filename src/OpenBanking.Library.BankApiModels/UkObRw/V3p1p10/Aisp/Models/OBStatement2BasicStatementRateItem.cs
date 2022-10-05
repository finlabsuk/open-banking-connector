// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Set of elements used to provide details of a generic rate related to the statement resource. </summary>
    public partial class OBStatement2BasicStatementRateItem
    {
        /// <summary> Initializes a new instance of OBStatement2BasicStatementRateItem. </summary>
        /// <param name="rate"> Rate associated with the statement rate type. </param>
        /// <param name="type"> Statement rate type, in a coded form. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="rate"/> or <paramref name="type"/> is null. </exception>
        public OBStatement2BasicStatementRateItem(string rate, string type)
        {
            if (rate == null)
            {
                throw new ArgumentNullException(nameof(rate));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Rate = rate;
            Type = type;
        }

        /// <summary> Rate associated with the statement rate type. </summary>
        public string Rate { get; }
        /// <summary> Statement rate type, in a coded form. </summary>
        public string Type { get; }
    }
}
