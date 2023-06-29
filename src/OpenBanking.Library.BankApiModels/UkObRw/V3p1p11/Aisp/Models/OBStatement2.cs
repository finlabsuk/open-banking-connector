// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> Provides further details on a statement resource. </summary>
    public partial class OBStatement2
    {
        /// <summary> Initializes a new instance of OBStatement2. </summary>
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
        internal OBStatement2(string accountId, OBExternalStatementType1CodeEnum type, DateTimeOffset startDateTime, DateTimeOffset endDateTime, DateTimeOffset creationDateTime)
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
            StatementBenefit = new ChangeTrackingList<OBStatement2StatementBenefitItem>();
            StatementFee = new ChangeTrackingList<OBStatement2StatementFeeItem>();
            StatementInterest = new ChangeTrackingList<OBStatement2StatementInterestItem>();
            StatementAmount = new ChangeTrackingList<OBStatement2StatementAmountItem>();
            StatementDateTime = new ChangeTrackingList<OBStatement2StatementDateTimeItem>();
            StatementRate = new ChangeTrackingList<OBStatement2StatementRateItem>();
            StatementValue = new ChangeTrackingList<OBStatement2StatementValueItem>();
        }

        /// <summary> Initializes a new instance of OBStatement2. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="statementId"> Unique identifier for the statement resource within an servicing institution. This identifier is both unique and immutable. </param>
        /// <param name="statementReference"> Unique reference for the statement. This reference may be optionally populated if available. </param>
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
        /// <param name="statementDescription"></param>
        /// <param name="statementBenefit"></param>
        /// <param name="statementFee"></param>
        /// <param name="statementInterest"></param>
        /// <param name="statementAmount"></param>
        /// <param name="statementDateTime"></param>
        /// <param name="statementRate"></param>
        /// <param name="statementValue"></param>
        /// <param name="totalValue"> Combined sum of all Amounts in the accounts base currency. </param>
        internal OBStatement2(string accountId, string statementId, string statementReference, OBExternalStatementType1CodeEnum type, DateTimeOffset startDateTime, DateTimeOffset endDateTime, DateTimeOffset creationDateTime, IReadOnlyList<string> statementDescription, IReadOnlyList<OBStatement2StatementBenefitItem> statementBenefit, IReadOnlyList<OBStatement2StatementFeeItem> statementFee, IReadOnlyList<OBStatement2StatementInterestItem> statementInterest, IReadOnlyList<OBStatement2StatementAmountItem> statementAmount, IReadOnlyList<OBStatement2StatementDateTimeItem> statementDateTime, IReadOnlyList<OBStatement2StatementRateItem> statementRate, IReadOnlyList<OBStatement2StatementValueItem> statementValue, OBStatement2TotalValue totalValue)
        {
            AccountId = accountId;
            StatementId = statementId;
            StatementReference = statementReference;
            Type = type;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            CreationDateTime = creationDateTime;
            StatementDescription = statementDescription;
            StatementBenefit = statementBenefit;
            StatementFee = statementFee;
            StatementInterest = statementInterest;
            StatementAmount = statementAmount;
            StatementDateTime = statementDateTime;
            StatementRate = statementRate;
            StatementValue = statementValue;
            TotalValue = totalValue;
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
        public IReadOnlyList<OBStatement2StatementBenefitItem> StatementBenefit { get; }
        /// <summary> Gets the statement fee. </summary>
        public IReadOnlyList<OBStatement2StatementFeeItem> StatementFee { get; }
        /// <summary> Gets the statement interest. </summary>
        public IReadOnlyList<OBStatement2StatementInterestItem> StatementInterest { get; }
        /// <summary> Gets the statement amount. </summary>
        public IReadOnlyList<OBStatement2StatementAmountItem> StatementAmount { get; }
        /// <summary> Gets the statement date time. </summary>
        public IReadOnlyList<OBStatement2StatementDateTimeItem> StatementDateTime { get; }
        /// <summary> Gets the statement rate. </summary>
        public IReadOnlyList<OBStatement2StatementRateItem> StatementRate { get; }
        /// <summary> Gets the statement value. </summary>
        public IReadOnlyList<OBStatement2StatementValueItem> StatementValue { get; }
        /// <summary> Combined sum of all Amounts in the accounts base currency. </summary>
        public OBStatement2TotalValue TotalValue { get; }
    }
}
