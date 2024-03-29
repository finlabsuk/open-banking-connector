// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> The OBScheduledPayment3Detail. </summary>
    public partial class OBScheduledPayment3Detail
    {
        /// <summary> Initializes a new instance of OBScheduledPayment3Detail. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="scheduledPaymentDateTime">
        /// The date on which the scheduled payment will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="scheduledType"> Specifies the scheduled payment date type requested. </param>
        /// <param name="instructedAmount">
        /// Amount of money to be moved between the debtor and creditor, before deduction of charges, expressed in the currency as ordered by the initiating party.
        /// Usage: This amount has to be transported unchanged through the transaction chain.
        /// </param>
        /// <param name="creditorAccount"> Provides the details to identify the beneficiary account. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/>, <paramref name="instructedAmount"/> or <paramref name="creditorAccount"/> is null. </exception>
        public OBScheduledPayment3Detail(string accountId, DateTimeOffset scheduledPaymentDateTime, OBExternalScheduleType1CodeEnum scheduledType, OBActiveOrHistoricCurrencyAndAmount1 instructedAmount, OBCashAccount51 creditorAccount)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (instructedAmount == null)
            {
                throw new ArgumentNullException(nameof(instructedAmount));
            }
            if (creditorAccount == null)
            {
                throw new ArgumentNullException(nameof(creditorAccount));
            }

            AccountId = accountId;
            ScheduledPaymentDateTime = scheduledPaymentDateTime;
            ScheduledType = scheduledType;
            InstructedAmount = instructedAmount;
            CreditorAccount = creditorAccount;
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> A unique and immutable identifier used to identify the scheduled payment resource. This identifier has no meaning to the account owner. </summary>
        public string ScheduledPaymentId { get; }
        /// <summary>
        /// The date on which the scheduled payment will be made.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset ScheduledPaymentDateTime { get; }
        /// <summary> Specifies the scheduled payment date type requested. </summary>
        public OBExternalScheduleType1CodeEnum ScheduledType { get; }
        /// <summary>
        /// Unique reference, as assigned by the creditor, to unambiguously refer to the payment transaction.
        /// Usage: If available, the initiating party should provide this reference in the structured remittance information, to enable reconciliation by the creditor upon receipt of the amount of money.
        /// If the business context requires the use of a creditor reference or a payment remit identification, and only one identifier can be passed through the end-to-end chain, the creditor&apos;s reference or payment remittance identification should be quoted in the end-to-end transaction identification.
        /// </summary>
        public string Reference { get; }
        /// <summary> A reference value provided by the PSU to the PISP while setting up the scheduled payment. </summary>
        public string DebtorReference { get; }
        /// <summary>
        /// Amount of money to be moved between the debtor and creditor, before deduction of charges, expressed in the currency as ordered by the initiating party.
        /// Usage: This amount has to be transported unchanged through the transaction chain.
        /// </summary>
        public OBActiveOrHistoricCurrencyAndAmount1 InstructedAmount { get; }
        /// <summary>
        /// Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account.
        /// This is the servicer of the beneficiary account.
        /// </summary>
        public OBBranchAndFinancialInstitutionIdentification51 CreditorAgent { get; }
        /// <summary> Provides the details to identify the beneficiary account. </summary>
        public OBCashAccount51 CreditorAccount { get; }
    }
}
