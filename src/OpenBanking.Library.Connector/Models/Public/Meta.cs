// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public
{
    [OpenBankingEquivalent(typeof(ObModels.PaymentInitiation.V3p1p1.Model.Meta))]
    [OpenBankingEquivalent(typeof(ObModels.PaymentInitiation.V3p1p2.Model.Meta))]
    [SourceApiEquivalent(typeof(ObModels.PaymentInitiation.V3p1p1.Model.Meta))]
    [SourceApiEquivalent(typeof(ObModels.PaymentInitiation.V3p1p2.Model.Meta))]
    public class Meta
    {
        /// <summary>
        ///     Gets or Sets TotalPages
        /// </summary>
        [JsonProperty("totalPages")]
        public int? TotalPages { get; set; }

        /// <summary>
        ///     All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses
        ///     must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>
        ///     All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses
        ///     must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </value>
        [JsonProperty("firstAvailableDateTime")]
        public DateTime? FirstAvailableDateTime { get; set; }

        /// <summary>
        ///     All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses
        ///     must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>
        ///     All dates in the JSON payloads are represented in ISO 8601 date-time format.  All date-time fields in responses
        ///     must include the timezone. An example is below: 2017-04-05T10:43:07+00:00
        /// </value>
        [JsonProperty("lastAvailableDateTime")]
        public DateTime? LastAvailableDateTime { get; set; }
    }
}
