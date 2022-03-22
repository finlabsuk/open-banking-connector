// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary>
    /// Unambiguous identification of the account to which credit and debit entries are made. The following fields are optional only for accounts that are switched:
    /// 
    ///   * Data.Currency  
    ///   * Data.AccountType  
    ///   * Data.AccountSubType
    /// 
    /// For all other accounts, the fields must be populated by the ASPSP.
    /// </summary>
    public partial class OBAccount6
    {
        /// <summary> Initializes a new instance of OBAccount6. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="accountId"/> is null. </exception>
        public OBAccount6(string accountId)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            AccountId = accountId;
            Account = new ChangeTrackingList<OBAccount6AccountItem>();
        }

        /// <summary> Initializes a new instance of OBAccount6. </summary>
        /// <param name="accountId"> A unique and immutable identifier used to identify the account resource. This identifier has no meaning to the account owner. </param>
        /// <param name="status"> Specifies the status of account resource in code form. </param>
        /// <param name="statusUpdateDateTime">
        /// Date and time at which the resource status was updated.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="currency">
        /// Identification of the currency in which the account is held. 
        /// Usage: Currency should only be used in case one and the same account number covers several currencies
        /// and the initiating party needs to identify which currency needs to be used for settlement on the account.
        /// </param>
        /// <param name="accountType"> Specifies the type of account (personal or business). </param>
        /// <param name="accountSubType"> Specifies the sub type of account (product family group). </param>
        /// <param name="description"> Specifies the description of the account type. </param>
        /// <param name="nickname"> The nickname of the account, assigned by the account owner in order to provide an additional means of identification of the account. </param>
        /// <param name="openingDate">
        /// Date on which the account and related basic services are effectively operational for the account owner.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="maturityDate">
        /// Maturity date of the account.All dates in the JSON payloads are represented in ISO 8601 date-time format. 
        /// All date-time fields in responses must include the timezone. An example is below:
        /// 2017-04-05T10:43:07+00:00
        /// </param>
        /// <param name="switchStatus"> Specifies the switch status for the account, in a coded form. </param>
        /// <param name="account"></param>
        /// <param name="servicer"> Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account. </param>
        [JsonConstructor]
        public OBAccount6(string accountId, OBAccountStatus1CodeEnum? status, DateTimeOffset? statusUpdateDateTime, string currency, OBExternalAccountType1CodeEnum? accountType, OBExternalAccountSubType1CodeEnum? accountSubType, string description, string nickname, DateTimeOffset? openingDate, DateTimeOffset? maturityDate, string switchStatus, IReadOnlyList<OBAccount6AccountItem> account, OBBranchAndFinancialInstitutionIdentification50 servicer)
        {
            AccountId = accountId;
            Status = status;
            StatusUpdateDateTime = statusUpdateDateTime;
            Currency = currency;
            AccountType = accountType;
            AccountSubType = accountSubType;
            Description = description;
            Nickname = nickname;
            OpeningDate = openingDate;
            MaturityDate = maturityDate;
            SwitchStatus = switchStatus;
            Account = account;
            Servicer = servicer;
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
        public OBExternalAccountType1CodeEnum? AccountType { get; }
        /// <summary> Specifies the sub type of account (product family group). </summary>
        public OBExternalAccountSubType1CodeEnum? AccountSubType { get; }
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
        /// <summary> Gets the account. </summary>
        public IReadOnlyList<OBAccount6AccountItem> Account { get; }
        /// <summary> Party that manages the account on behalf of the account owner, that is manages the registration and booking of entries on the account, calculates balances on the account and provides information about the account. </summary>
        public OBBranchAndFinancialInstitutionIdentification50 Servicer { get; }
    }
}
