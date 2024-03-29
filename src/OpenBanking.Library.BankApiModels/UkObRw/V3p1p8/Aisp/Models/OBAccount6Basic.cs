// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> Unambiguous identification of the account to which credit and debit entries are made. </summary>
    public partial class OBAccount6Basic
    {
        /// <summary> Initializes a new instance of OBAccount6Basic. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="currency">
        /// Identification of the currency in which the account is held. 
        /// Usage: Currency should only be used in case one and the same account number covers several currencies
        /// and the initiating party needs to identify which currency needs to be used for settlement on the account.
        /// </param>
        /// <param name="accountType"> Specifies the type of account (personal or business). </param>
        /// <param name="accountSubType"> Specifies the sub type of account (product family group). </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/> or <paramref name="currency"/> is null. </exception>
        public OBAccount6Basic(string accountId, string currency, OBExternalAccountType1CodeEnum accountType, OBExternalAccountSubType1CodeEnum accountSubType)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            if (currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            AccountId = accountId;
            Currency = currency;
            AccountType = accountType;
            AccountSubType = accountSubType;
        }

        /// <summary> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </summary>
        public string AccountId { get; }
        /// <summary> Specifies the status of account resource in code form. </summary>
        public OBAccountStatus1CodeEnum? Status { get; }
        /// <summary>
        /// Date and time at which the resource status was updated.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? StatusUpdateDateTime { get; }
        /// <summary>
        /// Identification of the currency in which the account is held. 
        /// Usage: Currency should only be used in case one and the same account number covers several currencies
        /// and the initiating party needs to identify which currency needs to be used for settlement on the account.
        /// </summary>
        public string Currency { get; }
        /// <summary> Specifies the type of account (personal or business). </summary>
        public OBExternalAccountType1CodeEnum AccountType { get; }
        /// <summary> Specifies the sub type of account (product family group). </summary>
        public OBExternalAccountSubType1CodeEnum AccountSubType { get; }
        /// <summary> Specifies the description of the account type. </summary>
        public string Description { get; }
        /// <summary> The nickname of the account, assigned by the account owner in order to provide an additional means of identification of the account. </summary>
        public string Nickname { get; }
        /// <summary>
        /// Date on which the account and related basic services are effectively operational for the account owner.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? OpeningDate { get; }
        /// <summary>
        /// Maturity date of the account.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </summary>
        public DateTimeOffset? MaturityDate { get; }
        /// <summary> Specifies the switch status for the account, in a coded form. </summary>
        public string SwitchStatus { get; }
    }
}
