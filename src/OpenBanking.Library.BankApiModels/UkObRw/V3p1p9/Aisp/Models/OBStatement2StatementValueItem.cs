// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Set of elements used to provide details of a generic number value related to the statement resource. </summary>
    public partial class OBStatement2StatementValueItem
    {
        /// <summary> Initializes a new instance of OBStatement2StatementValueItem. </summary>
        /// <param name="value"> Value associated with the statement value type. </param>
        /// <param name="type"> Statement value type, in a coded form. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> or <paramref name="type"/> is null. </exception>
        internal OBStatement2StatementValueItem(string value, string type)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Value = value;
            Type = type;
        }

        /// <summary> Value associated with the statement value type. </summary>
        public string Value { get; }
        /// <summary> Statement value type, in a coded form. </summary>
        public string Type { get; }
    }
}
