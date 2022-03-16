// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Set of elements used to define the balance details. </summary>
    public partial class OBReadBalance1DataBalanceItem
    {
        /// <summary> Initializes a new instance of OBReadBalance1DataBalanceItem. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the balance is a credit or a debit balance. 
        /// Usage: A zero balance is considered to be a credit balance.
        /// </param>
        /// <param name="type"> Balance type, in a coded form. </param>
        /// <param name="dateTime">
        /// Indicates the date (and time) of the balance.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="amount"> Amount of money of the cash balance. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/> or <paramref name="amount"/> is null. </exception>
        internal OBReadBalance1DataBalanceItem(string accountId, OBCreditDebitCode2Enum creditDebitIndicator, OBBalanceType1CodeEnum type, DateTimeOffset dateTime, OBReadBalance1DataBalanceItemAmount amount)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (amount == null)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            AccountId = accountId;
            CreditDebitIndicator = creditDebitIndicator;
            Type = type;
            DateTime = dateTime;
            Amount = amount;
            CreditLine = new ChangeTrackingList<OBReadBalance1DataBalancePropertiesItemsItem>();
        }

        /// <summary> Initializes a new instance of OBReadBalance1DataBalanceItem. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the balance is a credit or a debit balance. 
        /// Usage: A zero balance is considered to be a credit balance.
        /// </param>
        /// <param name="type"> Balance type, in a coded form. </param>
        /// <param name="dateTime">
        /// Indicates the date (and time) of the balance.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="amount"> Amount of money of the cash balance. </param>
        /// <param name="creditLine"></param>
        internal OBReadBalance1DataBalanceItem(string accountId, OBCreditDebitCode2Enum creditDebitIndicator, OBBalanceType1CodeEnum type, DateTimeOffset dateTime, OBReadBalance1DataBalanceItemAmount amount, IReadOnlyList<OBReadBalance1DataBalancePropertiesItemsItem> creditLine)
        {
            AccountId = accountId;
            CreditDebitIndicator = creditDebitIndicator;
            Type = type;
            DateTime = dateTime;
            Amount = amount;
            CreditLine = creditLine;
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary>
        /// Indicates whether the balance is a credit or a debit balance. 
        /// Usage: A zero balance is considered to be a credit balance.
        /// </summary>
        public OBCreditDebitCode2Enum CreditDebitIndicator { get; }
        /// <summary> Balance type, in a coded form. </summary>
        public OBBalanceType1CodeEnum Type { get; }
        /// <summary>
        /// Indicates the date (and time) of the balance.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset DateTime { get; }
        /// <summary> Amount of money of the cash balance. </summary>
        public OBReadBalance1DataBalanceItemAmount Amount { get; }
        /// <summary> Gets the credit line. </summary>
        public IReadOnlyList<OBReadBalance1DataBalancePropertiesItemsItem> CreditLine { get; }
    }
}
