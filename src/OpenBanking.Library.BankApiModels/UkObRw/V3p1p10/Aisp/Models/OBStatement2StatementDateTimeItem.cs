// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Set of elements used to provide details of a generic date time for the statement resource. </summary>
    public partial class OBStatement2StatementDateTimeItem
    {
        /// <summary> Initializes a new instance of OBStatement2StatementDateTimeItem. </summary>
        /// <param name="dateTime">
        /// Date and time associated with the date time type.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="type"> Date time type, in a coded form. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> is null. </exception>
        public OBStatement2StatementDateTimeItem(DateTimeOffset dateTime, string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            DateTime = dateTime;
            Type = type;
        }

        /// <summary>
        /// Date and time associated with the date time type.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset DateTime { get; }
        /// <summary> Date time type, in a coded form. </summary>
        public string Type { get; }
    }
}
