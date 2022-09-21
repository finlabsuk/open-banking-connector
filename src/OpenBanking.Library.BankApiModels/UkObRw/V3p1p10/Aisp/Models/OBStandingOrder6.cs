// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> The OBStandingOrder6. </summary>
    public partial class OBStandingOrder6
    {
        /// <summary> Initializes a new instance of OBStandingOrder6. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="frequency">
        /// Individual Definitions:
        /// NotKnown - Not Known
        /// EvryDay - Every day
        /// EvryWorkgDay - Every working day
        /// IntrvlDay - An interval specified in number of calendar days (02 to 31)
        /// IntrvlWkDay - An interval specified in weeks (01 to 09), and the day within the week (01 to 07)
        /// WkInMnthDay - A monthly interval, specifying the week of the month (01 to 05) and day within the week (01 to 07)
        /// IntrvlMnthDay - An interval specified in months (between 01 to 06, 12, 24), specifying the day within the month (-05 to -01, 01 to 31)
        /// QtrDay - Quarterly (either ENGLISH, SCOTTISH, or RECEIVED)
        /// ENGLISH = Paid on the 25th March, 24th June, 29th September and 25th December.
        /// SCOTTISH = Paid on the 2nd February, 15th May, 1st August and 11th November.
        /// RECEIVED = Paid on the 20th March, 19th June, 24th September and 20th December.
        /// Individual Patterns:
        /// NotKnown (ScheduleCode)
        /// EvryDay (ScheduleCode)
        /// EvryWorkgDay (ScheduleCode)
        /// IntrvlDay:NoOfDay (ScheduleCode + NoOfDay)
        /// IntrvlWkDay:IntervalInWeeks:DayInWeek (ScheduleCode + IntervalInWeeks + DayInWeek)
        /// WkInMnthDay:WeekInMonth:DayInWeek (ScheduleCode + WeekInMonth + DayInWeek)
        /// IntrvlMnthDay:IntervalInMonths:DayInMonth (ScheduleCode + IntervalInMonths + DayInMonth)
        /// QtrDay: + either (ENGLISH, SCOTTISH or RECEIVED) ScheduleCode + QuarterDay
        /// The regular expression for this element combines five smaller versions for each permitted pattern. To aid legibility - the components are presented individually here:
        /// NotKnown
        /// EvryDay
        /// EvryWorkgDay
        /// IntrvlDay:((0[2-9])|([1-2][0-9])|3[0-1])
        /// IntrvlWkDay:0[1-9]:0[1-7]
        /// WkInMnthDay:0[1-5]:0[1-7]
        /// IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01])
        /// QtrDay:(ENGLISH|SCOTTISH|RECEIVED)
        /// Full Regular Expression:
        /// ^(NotKnown)$|^(EvryDay)$|^(EvryWorkgDay)$|^(IntrvlDay:((0[2-9])|([1-2][0-9])|3[0-1]))$|^(IntrvlWkDay:0[1-9]:0[1-7])$|^(WkInMnthDay:0[1-5]:0[1-7])$|^(IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]))$|^(QtrDay:(ENGLISH|SCOTTISH|RECEIVED))$
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/> or <paramref name="frequency"/> is null. </exception>
        internal OBStandingOrder6(string accountId, string frequency)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (frequency == null)
            {
                throw new ArgumentNullException(nameof(frequency));
            }

            AccountId = accountId;
            Frequency = frequency;
            SupplementaryData = new ChangeTrackingDictionary<string, object>();
        }

        /// <summary> Initializes a new instance of OBStandingOrder6. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="standingOrderId"> A unique and immutable identifier used to identify the standing order resource. This identifier has no meaning to the account owner. </param>
        /// <param name="frequency">
        /// Individual Definitions:
        /// NotKnown - Not Known
        /// EvryDay - Every day
        /// EvryWorkgDay - Every working day
        /// IntrvlDay - An interval specified in number of calendar days (02 to 31)
        /// IntrvlWkDay - An interval specified in weeks (01 to 09), and the day within the week (01 to 07)
        /// WkInMnthDay - A monthly interval, specifying the week of the month (01 to 05) and day within the week (01 to 07)
        /// IntrvlMnthDay - An interval specified in months (between 01 to 06, 12, 24), specifying the day within the month (-05 to -01, 01 to 31)
        /// QtrDay - Quarterly (either ENGLISH, SCOTTISH, or RECEIVED)
        /// ENGLISH = Paid on the 25th March, 24th June, 29th September and 25th December.
        /// SCOTTISH = Paid on the 2nd February, 15th May, 1st August and 11th November.
        /// RECEIVED = Paid on the 20th March, 19th June, 24th September and 20th December.
        /// Individual Patterns:
        /// NotKnown (ScheduleCode)
        /// EvryDay (ScheduleCode)
        /// EvryWorkgDay (ScheduleCode)
        /// IntrvlDay:NoOfDay (ScheduleCode + NoOfDay)
        /// IntrvlWkDay:IntervalInWeeks:DayInWeek (ScheduleCode + IntervalInWeeks + DayInWeek)
        /// WkInMnthDay:WeekInMonth:DayInWeek (ScheduleCode + WeekInMonth + DayInWeek)
        /// IntrvlMnthDay:IntervalInMonths:DayInMonth (ScheduleCode + IntervalInMonths + DayInMonth)
        /// QtrDay: + either (ENGLISH, SCOTTISH or RECEIVED) ScheduleCode + QuarterDay
        /// The regular expression for this element combines five smaller versions for each permitted pattern. To aid legibility - the components are presented individually here:
        /// NotKnown
        /// EvryDay
        /// EvryWorkgDay
        /// IntrvlDay:((0[2-9])|([1-2][0-9])|3[0-1])
        /// IntrvlWkDay:0[1-9]:0[1-7]
        /// WkInMnthDay:0[1-5]:0[1-7]
        /// IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01])
        /// QtrDay:(ENGLISH|SCOTTISH|RECEIVED)
        /// Full Regular Expression:
        /// ^(NotKnown)$|^(EvryDay)$|^(EvryWorkgDay)$|^(IntrvlDay:((0[2-9])|([1-2][0-9])|3[0-1]))$|^(IntrvlWkDay:0[1-9]:0[1-7])$|^(WkInMnthDay:0[1-5]:0[1-7])$|^(IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]))$|^(QtrDay:(ENGLISH|SCOTTISH|RECEIVED))$
        /// </param>
        /// <param name="reference">
        /// Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction.
        /// Usage: If available, the initiating party should provide this reference in the structured remittance information, to enable reconciliation by the creditor upon receipt of the amount of money.
        /// If the business context requires the use of a creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end chain, the creditor&apos;s reference or payment remittance identification should be quoted in the end-to-end transaction identification.
        /// </param>
        /// <param name="firstPaymentDateTime">
        /// The date on which the first payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="nextPaymentDateTime">
        /// The date on which the next payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="lastPaymentDateTime">
        /// The date on which the last (most recent) payment for a Standing Order schedule was made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="finalPaymentDateTime">
        /// The date on which the final payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="numberOfPayments"> Number of the payments that will be made in completing this frequency sequence including any executed since the sequence start date. </param>
        /// <param name="standingOrderStatusCode"> Specifies the status of the standing order in code form. </param>
        /// <param name="firstPaymentAmount"> The amount of the first Standing Order. </param>
        /// <param name="nextPaymentAmount"> The amount of the next Standing Order. </param>
        /// <param name="lastPaymentAmount"> The amount of the last (most recent) Standing Order instruction. </param>
        /// <param name="finalPaymentAmount"> The amount of the final Standing Order. </param>
        /// <param name="creditorAgent">
        /// Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account.
        /// This is the servicer of the beneficiary account.
        /// </param>
        /// <param name="creditorAccount"> Provides the details to identify the beneficiary account. </param>
        /// <param name="supplementaryData"> Additional information that can not be captured in the structured fields and/or any other specific block. </param>
        internal OBStandingOrder6(string accountId, string standingOrderId, string frequency, string reference, DateTimeOffset? firstPaymentDateTime, DateTimeOffset? nextPaymentDateTime, DateTimeOffset? lastPaymentDateTime, DateTimeOffset? finalPaymentDateTime, string numberOfPayments, OBExternalStandingOrderStatus1CodeEnum? standingOrderStatusCode, OBActiveOrHistoricCurrencyAndAmount2 firstPaymentAmount, OBActiveOrHistoricCurrencyAndAmount3 nextPaymentAmount, OBActiveOrHistoricCurrencyAndAmount11 lastPaymentAmount, OBActiveOrHistoricCurrencyAndAmount4 finalPaymentAmount, OBBranchAndFinancialInstitutionIdentification51 creditorAgent, OBCashAccount51 creditorAccount, IReadOnlyDictionary<string, object> supplementaryData)
        {
            AccountId = accountId;
            StandingOrderId = standingOrderId;
            Frequency = frequency;
            Reference = reference;
            FirstPaymentDateTime = firstPaymentDateTime;
            NextPaymentDateTime = nextPaymentDateTime;
            LastPaymentDateTime = lastPaymentDateTime;
            FinalPaymentDateTime = finalPaymentDateTime;
            NumberOfPayments = numberOfPayments;
            StandingOrderStatusCode = standingOrderStatusCode;
            FirstPaymentAmount = firstPaymentAmount;
            NextPaymentAmount = nextPaymentAmount;
            LastPaymentAmount = lastPaymentAmount;
            FinalPaymentAmount = finalPaymentAmount;
            CreditorAgent = creditorAgent;
            CreditorAccount = creditorAccount;
            SupplementaryData = supplementaryData;
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> A unique and immutable identifier used to identify the standing order resource. This identifier has no meaning to the account owner. </summary>
        public string StandingOrderId { get; }
        /// <summary>
        /// Individual Definitions:
        /// NotKnown - Not Known
        /// EvryDay - Every day
        /// EvryWorkgDay - Every working day
        /// IntrvlDay - An interval specified in number of calendar days (02 to 31)
        /// IntrvlWkDay - An interval specified in weeks (01 to 09), and the day within the week (01 to 07)
        /// WkInMnthDay - A monthly interval, specifying the week of the month (01 to 05) and day within the week (01 to 07)
        /// IntrvlMnthDay - An interval specified in months (between 01 to 06, 12, 24), specifying the day within the month (-05 to -01, 01 to 31)
        /// QtrDay - Quarterly (either ENGLISH, SCOTTISH, or RECEIVED)
        /// ENGLISH = Paid on the 25th March, 24th June, 29th September and 25th December.
        /// SCOTTISH = Paid on the 2nd February, 15th May, 1st August and 11th November.
        /// RECEIVED = Paid on the 20th March, 19th June, 24th September and 20th December.
        /// Individual Patterns:
        /// NotKnown (ScheduleCode)
        /// EvryDay (ScheduleCode)
        /// EvryWorkgDay (ScheduleCode)
        /// IntrvlDay:NoOfDay (ScheduleCode + NoOfDay)
        /// IntrvlWkDay:IntervalInWeeks:DayInWeek (ScheduleCode + IntervalInWeeks + DayInWeek)
        /// WkInMnthDay:WeekInMonth:DayInWeek (ScheduleCode + WeekInMonth + DayInWeek)
        /// IntrvlMnthDay:IntervalInMonths:DayInMonth (ScheduleCode + IntervalInMonths + DayInMonth)
        /// QtrDay: + either (ENGLISH, SCOTTISH or RECEIVED) ScheduleCode + QuarterDay
        /// The regular expression for this element combines five smaller versions for each permitted pattern. To aid legibility - the components are presented individually here:
        /// NotKnown
        /// EvryDay
        /// EvryWorkgDay
        /// IntrvlDay:((0[2-9])|([1-2][0-9])|3[0-1])
        /// IntrvlWkDay:0[1-9]:0[1-7]
        /// WkInMnthDay:0[1-5]:0[1-7]
        /// IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01])
        /// QtrDay:(ENGLISH|SCOTTISH|RECEIVED)
        /// Full Regular Expression:
        /// ^(NotKnown)$|^(EvryDay)$|^(EvryWorkgDay)$|^(IntrvlDay:((0[2-9])|([1-2][0-9])|3[0-1]))$|^(IntrvlWkDay:0[1-9]:0[1-7])$|^(WkInMnthDay:0[1-5]:0[1-7])$|^(IntrvlMnthDay:(0[1-6]|12|24):(-0[1-5]|0[1-9]|[12][0-9]|3[01]))$|^(QtrDay:(ENGLISH|SCOTTISH|RECEIVED))$
        /// </summary>
        public string Frequency { get; }
        /// <summary>
        /// Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction.
        /// Usage: If available, the initiating party should provide this reference in the structured remittance information, to enable reconciliation by the creditor upon receipt of the amount of money.
        /// If the business context requires the use of a creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end chain, the creditor&apos;s reference or payment remittance identification should be quoted in the end-to-end transaction identification.
        /// </summary>
        public string Reference { get; }
        /// <summary>
        /// The date on which the first payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? FirstPaymentDateTime { get; }
        /// <summary>
        /// The date on which the next payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? NextPaymentDateTime { get; }
        /// <summary>
        /// The date on which the last (most recent) payment for a Standing Order schedule was made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? LastPaymentDateTime { get; }
        /// <summary>
        /// The date on which the final payment for a Standing Order schedule will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? FinalPaymentDateTime { get; }
        /// <summary> Number of the payments that will be made in completing this frequency sequence including any executed since the sequence start date. </summary>
        public string NumberOfPayments { get; }
        /// <summary> Specifies the status of the standing order in code form. </summary>
        public OBExternalStandingOrderStatus1CodeEnum? StandingOrderStatusCode { get; }
        /// <summary> The amount of the first Standing Order. </summary>
        public OBActiveOrHistoricCurrencyAndAmount2 FirstPaymentAmount { get; }
        /// <summary> The amount of the next Standing Order. </summary>
        public OBActiveOrHistoricCurrencyAndAmount3 NextPaymentAmount { get; }
        /// <summary> The amount of the last (most recent) Standing Order instruction. </summary>
        public OBActiveOrHistoricCurrencyAndAmount11 LastPaymentAmount { get; }
        /// <summary> The amount of the final Standing Order. </summary>
        public OBActiveOrHistoricCurrencyAndAmount4 FinalPaymentAmount { get; }
        /// <summary>
        /// Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account.
        /// This is the servicer of the beneficiary account.
        /// </summary>
        public OBBranchAndFinancialInstitutionIdentification51 CreditorAgent { get; }
        /// <summary> Provides the details to identify the beneficiary account. </summary>
        public OBCashAccount51 CreditorAccount { get; }
        /// <summary> Additional information that can not be captured in the structured fields and/or any other specific block. </summary>
        public IReadOnlyDictionary<string, object> SupplementaryData { get; }
    }
}
