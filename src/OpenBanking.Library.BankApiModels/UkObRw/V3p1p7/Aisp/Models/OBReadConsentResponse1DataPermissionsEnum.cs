// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Specifies the Open Banking account access data types. This is a list of the data clusters being consented by the PSU, and requested for authorisation with the ASPSP. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBReadConsentResponse1DataPermissionsEnum
    {
        /// <summary> ReadAccountsBasic. </summary>
        [EnumMember(Value = "ReadAccountsBasic")]
        ReadAccountsBasic,
        /// <summary> ReadAccountsDetail. </summary>
        [EnumMember(Value = "ReadAccountsDetail")]
        ReadAccountsDetail,
        /// <summary> ReadBalances. </summary>
        [EnumMember(Value = "ReadBalances")]
        ReadBalances,
        /// <summary> ReadBeneficiariesBasic. </summary>
        [EnumMember(Value = "ReadBeneficiariesBasic")]
        ReadBeneficiariesBasic,
        /// <summary> ReadBeneficiariesDetail. </summary>
        [EnumMember(Value = "ReadBeneficiariesDetail")]
        ReadBeneficiariesDetail,
        /// <summary> ReadDirectDebits. </summary>
        [EnumMember(Value = "ReadDirectDebits")]
        ReadDirectDebits,
        /// <summary> ReadOffers. </summary>
        [EnumMember(Value = "ReadOffers")]
        ReadOffers,
        /// <summary> ReadPAN. </summary>
        [EnumMember(Value = "ReadPAN")]
        ReadPAN,
        /// <summary> ReadParty. </summary>
        [EnumMember(Value = "ReadParty")]
        ReadParty,
        /// <summary> ReadPartyPSU. </summary>
        [EnumMember(Value = "ReadPartyPSU")]
        ReadPartyPSU,
        /// <summary> ReadProducts. </summary>
        [EnumMember(Value = "ReadProducts")]
        ReadProducts,
        /// <summary> ReadScheduledPaymentsBasic. </summary>
        [EnumMember(Value = "ReadScheduledPaymentsBasic")]
        ReadScheduledPaymentsBasic,
        /// <summary> ReadScheduledPaymentsDetail. </summary>
        [EnumMember(Value = "ReadScheduledPaymentsDetail")]
        ReadScheduledPaymentsDetail,
        /// <summary> ReadStandingOrdersBasic. </summary>
        [EnumMember(Value = "ReadStandingOrdersBasic")]
        ReadStandingOrdersBasic,
        /// <summary> ReadStandingOrdersDetail. </summary>
        [EnumMember(Value = "ReadStandingOrdersDetail")]
        ReadStandingOrdersDetail,
        /// <summary> ReadStatementsBasic. </summary>
        [EnumMember(Value = "ReadStatementsBasic")]
        ReadStatementsBasic,
        /// <summary> ReadStatementsDetail. </summary>
        [EnumMember(Value = "ReadStatementsDetail")]
        ReadStatementsDetail,
        /// <summary> ReadTransactionsBasic. </summary>
        [EnumMember(Value = "ReadTransactionsBasic")]
        ReadTransactionsBasic,
        /// <summary> ReadTransactionsCredits. </summary>
        [EnumMember(Value = "ReadTransactionsCredits")]
        ReadTransactionsCredits,
        /// <summary> ReadTransactionsDebits. </summary>
        [EnumMember(Value = "ReadTransactionsDebits")]
        ReadTransactionsDebits,
        /// <summary> ReadTransactionsDetail. </summary>
        [EnumMember(Value = "ReadTransactionsDetail")]
        ReadTransactionsDetail
    }
}
