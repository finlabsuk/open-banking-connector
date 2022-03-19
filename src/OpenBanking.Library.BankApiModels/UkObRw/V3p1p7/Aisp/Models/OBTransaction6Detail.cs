// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Provides further details on an entry in the report. </summary>
    public partial class OBTransaction6Detail
    {
        /// <summary> Initializes a new instance of OBTransaction6Detail. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="creditDebitIndicator"> Indicates whether the transaction is a credit or a debit entry. </param>
        /// <param name="status"> Status of a transaction entry on the books of the account servicer. </param>
        /// <param name="bookingDateTime">
        /// Date and time when a transaction entry is posted to an account on the account servicer&apos;s books.
        /// Usage: Booking date is the expected booking date, unless the status is booked, in which case it is the actual booking date.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="amount"> Amount of money in the cash transaction entry. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/> or <paramref name="amount"/> is null. </exception>
        public OBTransaction6Detail(string accountId, OBCreditDebitCode1Enum creditDebitIndicator, OBEntryStatus1CodeEnum status, DateTimeOffset bookingDateTime, OBActiveOrHistoricCurrencyAndAmount9 amount)
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
            StatementReference = new ChangeTrackingList<string>();
            CreditDebitIndicator = creditDebitIndicator;
            Status = status;
            BookingDateTime = bookingDateTime;
            Amount = amount;
            SupplementaryData = new ChangeTrackingDictionary<string, object>();
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> Unique identifier for the transaction within an servicing institution. This identifier is both unique and immutable. </summary>
        public string TransactionId { get; }
        /// <summary> Unique reference for the transaction. This reference is optionally populated, and may as an example be the FPID in the Faster Payments context. </summary>
        public string TransactionReference { get; }
        /// <summary> Gets the statement reference. </summary>
        public IReadOnlyList<string> StatementReference { get; }
        /// <summary> Indicates whether the transaction is a credit or a debit entry. </summary>
        public OBCreditDebitCode1Enum CreditDebitIndicator { get; }
        /// <summary> Status of a transaction entry on the books of the account servicer. </summary>
        public OBEntryStatus1CodeEnum Status { get; }
        /// <summary> Specifies the Mutability of the Transaction record. </summary>
        public OBTransactionMutability1CodeEnum? TransactionMutability { get; }
        /// <summary>
        /// Date and time when a transaction entry is posted to an account on the account servicer&apos;s books.
        /// Usage: Booking date is the expected booking date, unless the status is booked, in which case it is the actual booking date.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset BookingDateTime { get; }
        /// <summary>
        /// Date and time at which assets become available to the account owner in case of a credit entry, or cease to be available to the account owner in case of a debit transaction entry.
        /// Usage: If transaction entry status is pending and value date is present, then the value date refers to an expected/requested value date.
        /// For transaction entries subject to availability/float and for which availability information is provided, the value date must not be used. In this case the availability component identifies the number of availability days.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? ValueDateTime { get; }
        /// <summary>
        /// Further details of the transaction. 
        /// This is the transaction narrative, which is unstructured text.
        /// </summary>
        public string TransactionInformation { get; }
        /// <summary> Information that locates and identifies a specific address for a transaction entry, that is presented in free format text. </summary>
        public string AddressLine { get; }
        /// <summary> Amount of money in the cash transaction entry. </summary>
        public OBActiveOrHistoricCurrencyAndAmount9 Amount { get; }
        /// <summary> Transaction charges to be paid by the charge bearer. </summary>
        public OBActiveOrHistoricCurrencyAndAmount10 ChargeAmount { get; }
        /// <summary> Set of elements used to provide details on the currency exchange. </summary>
        public OBCurrencyExchange5 CurrencyExchange { get; }
        /// <summary> Set of elements used to fully identify the type of underlying transaction resulting in an entry. </summary>
        public OBBankTransactionCodeStructure1 BankTransactionCode { get; }
        /// <summary> Set of elements to fully identify a proprietary bank transaction code. </summary>
        public ProprietaryBankTransactionCodeStructure1 ProprietaryBankTransactionCode { get; }
        /// <summary> Set of elements used to define the balance as a numerical representation of the net increases and decreases in an account after a transaction entry is applied to the account. </summary>
        public OBTransactionCashBalance Balance { get; }
        /// <summary> Details of the merchant involved in the transaction. </summary>
        public OBMerchantDetails1 MerchantDetails { get; }
        /// <summary> Financial institution servicing an account for the creditor. </summary>
        public OBBranchAndFinancialInstitutionIdentification61 CreditorAgent { get; }
        /// <summary> Unambiguous identification of the account of the creditor, in the case of a debit transaction. </summary>
        public OBCashAccount60 CreditorAccount { get; }
        /// <summary> Financial institution servicing an account for the debtor. </summary>
        public OBBranchAndFinancialInstitutionIdentification62 DebtorAgent { get; }
        /// <summary> Unambiguous identification of the account of the debtor, in the case of a crebit transaction. </summary>
        public OBCashAccount61 DebtorAccount { get; }
        /// <summary> Set of elements to describe the card instrument used in the transaction. </summary>
        public OBTransactionCardInstrument1 CardInstrument { get; }
        /// <summary> Additional information that can not be captured in the structured fields and/or any other specific block. </summary>
        public IReadOnlyDictionary<string, object> SupplementaryData { get; }
    }
}
