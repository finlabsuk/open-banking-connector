// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Set of elements used to provide details on the currency exchange. </summary>
    public partial class OBCurrencyExchange5
    {
        /// <summary> Initializes a new instance of OBCurrencyExchange5. </summary>
        /// <param name="sourceCurrency"> Currency from which an amount is to be converted in a currency conversion. </param>
        /// <param name="exchangeRate">
        /// Factor used to convert an amount from one currency into another. This reflects the price at which one currency was bought with another currency.
        /// Usage: ExchangeRate expresses the ratio between UnitCurrency and QuotedCurrency (ExchangeRate = UnitCurrency/QuotedCurrency).
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="sourceCurrency"/> is null. </exception>
        internal OBCurrencyExchange5(string sourceCurrency, float exchangeRate)
        {
            if (sourceCurrency == null)
            {
                throw new ArgumentNullException(nameof(sourceCurrency));
            }

            SourceCurrency = sourceCurrency;
            ExchangeRate = exchangeRate;
        }

        /// <summary> Initializes a new instance of OBCurrencyExchange5. </summary>
        /// <param name="sourceCurrency"> Currency from which an amount is to be converted in a currency conversion. </param>
        /// <param name="targetCurrency"> Currency into which an amount is to be converted in a currency conversion. </param>
        /// <param name="unitCurrency"> Currency in which the rate of exchange is expressed in a currency exchange. In the example 1GBP = xxxCUR, the unit currency is GBP. </param>
        /// <param name="exchangeRate">
        /// Factor used to convert an amount from one currency into another. This reflects the price at which one currency was bought with another currency.
        /// Usage: ExchangeRate expresses the ratio between UnitCurrency and QuotedCurrency (ExchangeRate = UnitCurrency/QuotedCurrency).
        /// </param>
        /// <param name="contractIdentification"> Unique identification to unambiguously identify the foreign exchange contract. </param>
        /// <param name="quotationDate">
        /// Date and time at which an exchange rate is quoted.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="instructedAmount"> Amount of money to be moved between the debtor and creditor, before deduction of charges, expressed in the currency as ordered by the initiating party. </param>
        internal OBCurrencyExchange5(string sourceCurrency, string targetCurrency, string unitCurrency, float exchangeRate, string contractIdentification, DateTimeOffset? quotationDate, OBCurrencyExchange5InstructedAmount instructedAmount)
        {
            SourceCurrency = sourceCurrency;
            TargetCurrency = targetCurrency;
            UnitCurrency = unitCurrency;
            ExchangeRate = exchangeRate;
            ContractIdentification = contractIdentification;
            QuotationDate = quotationDate;
            InstructedAmount = instructedAmount;
        }

        /// <summary> Currency from which an amount is to be converted in a currency conversion. </summary>
        public string SourceCurrency { get; }
        /// <summary> Currency into which an amount is to be converted in a currency conversion. </summary>
        public string TargetCurrency { get; }
        /// <summary> Currency in which the rate of exchange is expressed in a currency exchange. In the example 1GBP = xxxCUR, the unit currency is GBP. </summary>
        public string UnitCurrency { get; }
        /// <summary>
        /// Factor used to convert an amount from one currency into another. This reflects the price at which one currency was bought with another currency.
        /// Usage: ExchangeRate expresses the ratio between UnitCurrency and QuotedCurrency (ExchangeRate = UnitCurrency/QuotedCurrency).
        /// </summary>
        public float ExchangeRate { get; }
        /// <summary> Unique identification to unambiguously identify the foreign exchange contract. </summary>
        public string ContractIdentification { get; }
        /// <summary>
        /// Date and time at which an exchange rate is quoted.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? QuotationDate { get; }
        /// <summary> Amount of money to be moved between the debtor and creditor, before deduction of charges, expressed in the currency as ordered by the initiating party. </summary>
        public OBCurrencyExchange5InstructedAmount InstructedAmount { get; }
    }
}
