// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> The OBReadConsent1Data. </summary>
    [TargetApiEquivalent(typeof(V3p1p7.Aisp.Models.OBReadConsent1Data))]
    public partial class OBReadConsent1Data
    {
        /// <summary> Initializes a new instance of OBReadConsent1Data. </summary>
        /// <param name="permissions"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="permissions"/> is null. </exception>
        public OBReadConsent1Data(IEnumerable<OBReadConsent1DataPermissionsEnum> permissions)
        {
            if (permissions == null)
            {
                throw new ArgumentNullException(nameof(permissions));
            }

            Permissions = permissions.ToList();
        }

        /// <summary> Gets the permissions. </summary>
        public IList<OBReadConsent1DataPermissionsEnum> Permissions { get; }
        /// <summary>
        /// Specified date and time the permissions will expire.
        /// If this is not populated, the permissions will be open ended.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? ExpirationDateTime { get; set; }
        /// <summary>
        /// Specified start date and time for the transaction query period.
        /// If this is not populated, the start date will be open ended, and data will be returned from the earliest available transaction.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? TransactionFromDateTime { get; set; }
        /// <summary>
        /// Specified end date and time for the transaction query period.
        /// If this is not populated, the end date will be open ended, and data will be returned to the latest available transaction.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? TransactionToDateTime { get; set; }
        
        /// <summary>
        /// Extra property used with Monzo
        /// </summary>
        [JsonProperty(PropertyName = "SupplementaryData")]
        public IDictionary<string, object> SupplementaryData { get; set; }

    }
}
