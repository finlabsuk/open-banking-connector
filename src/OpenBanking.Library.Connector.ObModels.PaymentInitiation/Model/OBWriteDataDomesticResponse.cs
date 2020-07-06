// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    [OpenBankingEquivalent(typeof(OBWriteDataDomesticResponse2))]
    [SourceApiEquivalent(typeof(OBWriteDataDomesticResponse2))]
    public class OBWriteDataDomesticResponse
    {
        [JsonProperty("status")]
        public OBTransactionIndividualStatus1Code Status { get; set; }

        [JsonProperty("domesticPaymentId")]
        public string DomesticPaymentId { get; set; }

        /// <summary>
        ///     OB: Unique identification as assigned by the ASPSP to uniquely identify the consent resource.
        /// </summary>
        /// <value>OB: Unique identification as assigned by the ASPSP to uniquely identify the consent resource.</value>
        [JsonProperty("consentId")]
        public string ConsentId { get; set; }

        /// <summary>
        ///     Date and time at which the message was created. All dates in the JSON payloads are represented in ISO 8601
        ///     date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>
        ///     Date and time at which the message was created. All dates in the JSON payloads are represented in ISO 8601
        ///     date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </value>
        [JsonProperty("creationDateTime")]
        public DateTime? CreationDateTime { get; set; }


        /// <summary>
        ///     Date and time at which the resource status was updated. All dates in the JSON payloads are represented in ISO 8601
        ///     date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>
        ///     Date and time at which the resource status was updated. All dates in the JSON payloads are represented in ISO
        ///     8601 date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </value>
        [JsonProperty("statusUpdateDateTime")]
        public DateTime? StatusUpdateDateTime { get; set; }

        /// <summary>
        ///     Expected execution date and time for the payment resource. All dates in the JSON payloads are represented in ISO
        ///     8601 date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>
        ///     Expected execution date and time for the payment resource. All dates in the JSON payloads are represented in ISO
        ///     8601 date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </value>
        [JsonProperty("expectedExecutionDateTime")]
        public DateTime? ExpectedExecutionDateTime { get; set; }

        /// <summary>
        ///     Expected settlement date and time for the payment resource. All dates in the JSON payloads are represented in ISO
        ///     8601 date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </summary>
        /// <value>
        ///     Expected settlement date and time for the payment resource. All dates in the JSON payloads are represented in
        ///     ISO 8601 date-time format.  All date-time fields in responses must include the timezone. An example is below:
        ///     2017-04-05T10:43:07+00:00
        /// </value>
        [JsonProperty("expectedSettlementDateTime")]
        public DateTime? ExpectedSettlementDateTime { get; set; }

        /// <summary>
        ///     Set of elements used to provide details of a charge for the payment initiation.
        /// </summary>
        /// <value>Set of elements used to provide details of a charge for the payment initiation.</value>
        [JsonProperty("charges")]
        public List<OBCharge> Charges { get; set; }

        /// <summary>
        ///     Gets or Sets Initiation
        /// </summary>
        [JsonProperty("initiation")]
        public OBDomestic Initiation { get; set; }

        /// <summary>
        ///     Gets or Sets MultiAuthorisation
        /// </summary>
        [JsonProperty("multiAuthorisation")]
        public OBMultiAuthorisation MultiAuthorisation { get; set; }
    }
}
