// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Provides further details on a statement resource. </summary>
    internal partial class OBStatement2Basic
    {
        /// <summary> Initializes a new instance of OBStatement2Basic. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="type"> Statement type, in a coded form. </param>
        /// <param name="startDateTime">
        /// Date and time at which the statement period starts.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="endDateTime">
        /// Date and time at which the statement period ends.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="creationDateTime">
        /// Date and time at which the resource was created.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/> is null. </exception>
        internal OBStatement2Basic(string accountId, OBExternalStatementType1CodeEnum type, DateTimeOffset startDateTime, DateTimeOffset endDateTime, DateTimeOffset creationDateTime)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            AccountId = accountId;
            Type = type;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            CreationDateTime = creationDateTime;
            StatementDescription = new ChangeTrackingList<string>();
            StatementBenefit = new ChangeTrackingList<OBStatement2BasicStatementBenefitItem>();
            StatementFee = new ChangeTrackingList<OBStatement2BasicStatementFeeItem>();
            StatementInterest = new ChangeTrackingList<OBStatement2BasicStatementInterestItem>();
            StatementDateTime = new ChangeTrackingList<OBStatement2BasicStatementDateTimeItem>();
            StatementRate = new ChangeTrackingList<OBStatement2BasicStatementRateItem>();
            StatementValue = new ChangeTrackingList<OBStatement2BasicStatementValueItem>();
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> Unique identifier for the statement resource within an servicing institution. This identifier is both unique and immutable. </summary>
        public string StatementId { get; }
        /// <summary> Unique reference for the statement. This reference may be optionally populated if available. </summary>
        public string StatementReference { get; }
        /// <summary> Statement type, in a coded form. </summary>
        public OBExternalStatementType1CodeEnum Type { get; }
        /// <summary>
        /// Date and time at which the statement period starts.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset StartDateTime { get; }
        /// <summary>
        /// Date and time at which the statement period ends.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset EndDateTime { get; }
        /// <summary>
        /// Date and time at which the resource was created.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset CreationDateTime { get; }
        /// <summary> Gets the statement description. </summary>
        public IReadOnlyList<string> StatementDescription { get; }
        /// <summary> Gets the statement benefit. </summary>
        public IReadOnlyList<OBStatement2BasicStatementBenefitItem> StatementBenefit { get; }
        /// <summary> Gets the statement fee. </summary>
        public IReadOnlyList<OBStatement2BasicStatementFeeItem> StatementFee { get; }
        /// <summary> Gets the statement interest. </summary>
        public IReadOnlyList<OBStatement2BasicStatementInterestItem> StatementInterest { get; }
        /// <summary> Gets the statement date time. </summary>
        public IReadOnlyList<OBStatement2BasicStatementDateTimeItem> StatementDateTime { get; }
        /// <summary> Gets the statement rate. </summary>
        public IReadOnlyList<OBStatement2BasicStatementRateItem> StatementRate { get; }
        /// <summary> Gets the statement value. </summary>
        public IReadOnlyList<OBStatement2BasicStatementValueItem> StatementValue { get; }
    }
}
