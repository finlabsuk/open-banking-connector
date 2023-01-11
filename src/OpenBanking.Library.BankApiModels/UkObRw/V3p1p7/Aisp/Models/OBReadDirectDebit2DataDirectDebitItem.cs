// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Account to or from which a cash entry is made. </summary>
    public partial class OBReadDirectDebit2DataDirectDebitItem
    {
        /// <summary> Initializes a new instance of OBReadDirectDebit2DataDirectDebitItem. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="mandateIdentification"> Direct Debit reference. For AUDDIS service users provide Core Reference. For non AUDDIS service users provide Core reference if possible or last used reference. </param>
        /// <param name="name"> Name of Service User. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/>, <paramref name="mandateIdentification"/> or <paramref name="name"/> is null. </exception>
        public OBReadDirectDebit2DataDirectDebitItem(string accountId, string mandateIdentification, string name)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (mandateIdentification == null)
            {
                throw new ArgumentNullException(nameof(mandateIdentification));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            AccountId = accountId;
            MandateIdentification = mandateIdentification;
            Name = name;
        }

        /// <summary> Initializes a new instance of OBReadDirectDebit2DataDirectDebitItem. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="directDebitId"> A unique and immutable identifier used to identify the direct debit resource. This identifier has no meaning to the account owner. </param>
        /// <param name="mandateIdentification"> Direct Debit reference. For AUDDIS service users provide Core Reference. For non AUDDIS service users provide Core reference if possible or last used reference. </param>
        /// <param name="directDebitStatusCode"> Specifies the status of the direct debit in code form. </param>
        /// <param name="name"> Name of Service User. </param>
        /// <param name="previousPaymentDateTime">
        /// Date of most recent direct debit collection.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="frequency"> Regularity with which direct debit instructions are to be created and processed. </param>
        /// <param name="previousPaymentAmount"> The amount of the most recent direct debit collection. </param>
        [JsonConstructor]
        public OBReadDirectDebit2DataDirectDebitItem(string accountId, string directDebitId, string mandateIdentification, OBExternalDirectDebitStatus1CodeEnum? directDebitStatusCode, string name, DateTimeOffset? previousPaymentDateTime, string frequency, OBActiveOrHistoricCurrencyAndAmount0 previousPaymentAmount)
        {
            AccountId = accountId;
            DirectDebitId = directDebitId;
            MandateIdentification = mandateIdentification;
            DirectDebitStatusCode = directDebitStatusCode;
            Name = name;
            PreviousPaymentDateTime = previousPaymentDateTime;
            Frequency = frequency;
            PreviousPaymentAmount = previousPaymentAmount;
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> A unique and immutable identifier used to identify the direct debit resource. This identifier has no meaning to the account owner. </summary>
        public string DirectDebitId { get; }
        /// <summary> Direct Debit reference. For AUDDIS service users provide Core Reference. For non AUDDIS service users provide Core reference if possible or last used reference. </summary>
        public string MandateIdentification { get; }
        /// <summary> Specifies the status of the direct debit in code form. </summary>
        public OBExternalDirectDebitStatus1CodeEnum? DirectDebitStatusCode { get; }
        /// <summary> Name of Service User. </summary>
        public string Name { get; }
        /// <summary>
        /// Date of most recent direct debit collection.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? PreviousPaymentDateTime { get; }
        /// <summary> Regularity with which direct debit instructions are to be created and processed. </summary>
        public string Frequency { get; }
        /// <summary> The amount of the most recent direct debit collection. </summary>
        public OBActiveOrHistoricCurrencyAndAmount0 PreviousPaymentAmount { get; }
    }
}
