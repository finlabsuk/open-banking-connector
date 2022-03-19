// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Meta Data relevant to the payload. </summary>
    public partial class Meta
    {
        /// <summary> Initializes a new instance of Meta. </summary>
        public Meta()
        {
        }

        /// <summary> Initializes a new instance of Meta. </summary>
        /// <param name="totalPages"></param>
        /// <param name="firstAvailableDateTime">
        /// All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="lastAvailableDateTime">
        /// All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        public Meta(int? totalPages, DateTimeOffset? firstAvailableDateTime, DateTimeOffset? lastAvailableDateTime)
        {
            TotalPages = totalPages;
            FirstAvailableDateTime = firstAvailableDateTime;
            LastAvailableDateTime = lastAvailableDateTime;
        }

        /// <summary> Gets the total pages. </summary>
        public int? TotalPages { get; }
        /// <summary>
        /// All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? FirstAvailableDateTime { get; }
        /// <summary>
        /// All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? LastAvailableDateTime { get; }
    }
}
