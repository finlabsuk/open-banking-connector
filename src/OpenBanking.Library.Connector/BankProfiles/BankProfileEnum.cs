// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

/// <summary>
///     Banks for which bank profiles have been created. Called BankProfileEnum to avoid confusion with the associated
///     class BankProfile.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum BankProfileEnum
{
    [EnumMember(Value = "Barclays_Sandbox")]
    Barclays_Sandbox,

    [EnumMember(Value = "Barclays_Personal")]
    Barclays_Personal,

    [EnumMember(Value = "Barclays_Wealth")]
    Barclays_Wealth,

    [EnumMember(Value = "Barclays_Barclaycard")]
    Barclays_Barclaycard,

    [EnumMember(Value = "Barclays_Business")]
    Barclays_Business,

    [EnumMember(Value = "Barclays_Corporate")]
    Barclays_Corporate,

    [EnumMember(Value = "Barclays_BarclaycardCommercialPayments")]
    Barclays_BarclaycardCommercialPayments,

    [EnumMember(Value = "Chase_Chase")]
    Chase_Chase,

    [EnumMember(Value = "Cooperative_Cooperative")]
    Cooperative_Cooperative,

    [EnumMember(Value = "Cooperative_CooperativeSandbox")]
    Cooperative_CooperativeSandbox,

    [EnumMember(Value = "Cooperative_Smile")]
    Cooperative_Smile,

    [EnumMember(Value = "Danske_Sandbox")]
    Danske_Sandbox,

    [EnumMember(Value = "Hsbc_FirstDirect")]
    Hsbc_FirstDirect,

    [EnumMember(Value = "Hsbc_Sandbox")]
    Hsbc_Sandbox,

    [EnumMember(Value = "Hsbc_UkBusiness")]
    Hsbc_UkBusiness,

    [EnumMember(Value = "Hsbc_UkKinetic")]
    Hsbc_UkKinetic,

    [EnumMember(Value = "Hsbc_UkPersonal")]
    Hsbc_UkPersonal,

    [EnumMember(Value = "Hsbc_HsbcNetUk")]
    Hsbc_HsbcNetUk,

    [EnumMember(Value = "Lloyds_Sandbox")]
    Lloyds_Sandbox,

    [EnumMember(Value = "Lloyds_LloydsPersonal")]
    Lloyds_LloydsPersonal,

    [EnumMember(Value = "Lloyds_LloydsBusiness")]
    Lloyds_LloydsBusiness,

    [EnumMember(Value = "Lloyds_LloydsCommerical")]
    Lloyds_LloydsCommerical,

    [EnumMember(Value = "Lloyds_HalifaxPersonal")]
    Lloyds_HalifaxPersonal,

    [EnumMember(Value = "Lloyds_BankOfScotlandPersonal")]
    Lloyds_BankOfScotlandPersonal,

    [EnumMember(Value = "Lloyds_BankOfScotlandBusiness")]
    Lloyds_BankOfScotlandBusiness,

    [EnumMember(Value = "Lloyds_BankOfScotlandCommerical")]
    Lloyds_BankOfScotlandCommerical,

    [EnumMember(Value = "Lloyds_MbnaPersonal")]
    Lloyds_MbnaPersonal,

    [EnumMember(Value = "Monzo_Monzo")]
    Monzo_Monzo,

    [EnumMember(Value = "Monzo_Sandbox")]
    Monzo_Sandbox,

    [EnumMember(Value = "Nationwide_Nationwide")]
    Nationwide_Nationwide,

    [EnumMember(Value = "NatWest_NatWestSandbox")]
    NatWest_NatWestSandbox,

    [EnumMember(Value = "NatWest_NatWest")]
    NatWest_NatWest,

    [EnumMember(Value = "NatWest_NatWestBankline")]
    NatWest_NatWestBankline,

    [EnumMember(Value = "NatWest_NatWestClearSpend")]
    NatWest_NatWestClearSpend,

    [EnumMember(Value = "NatWest_RoyalBankOfScotlandSandbox")]
    NatWest_RoyalBankOfScotlandSandbox,

    [EnumMember(Value = "NatWest_RoyalBankOfScotland")]
    NatWest_RoyalBankOfScotland,

    [EnumMember(Value = "NatWest_RoyalBankOfScotlandBankline")]
    NatWest_RoyalBankOfScotlandBankline,

    [EnumMember(Value = "NatWest_RoyalBankOfScotlandClearSpend")]
    NatWest_RoyalBankOfScotlandClearSpend,

    [EnumMember(Value = "NatWest_TheOne")]
    NatWest_TheOne,

    [EnumMember(Value = "NatWest_NatWestOne")]
    NatWest_NatWestOne,

    [EnumMember(Value = "NatWest_VirginOne")]
    NatWest_VirginOne,

    [EnumMember(Value = "NatWest_UlsterBankNI")]
    NatWest_UlsterBankNi,

    [EnumMember(Value = "NatWest_UlsterBankNiBankline")]
    NatWest_UlsterBankNiBankline,

    [EnumMember(Value = "NatWest_UlsterBankNiClearSpend")]
    NatWest_UlsterBankNiClearSpend,

    [EnumMember(Value = "NatWest_Mettle")]
    NatWest_Mettle,

    [EnumMember(Value = "NatWest_Coutts")]
    NatWest_Coutts,

    [EnumMember(Value = "Obie_Model2023")]
    Obie_Model2023,

    [EnumMember(Value = "Revolut_Revolut")]
    Revolut_Revolut,

    [EnumMember(Value = "Santander_Santander")]
    Santander_Santander,

    [EnumMember(Value = "Santander_Personal")]
    Santander_Personal,

    [EnumMember(Value = "Santander_Business")]
    Santander_Business,

    [EnumMember(Value = "Santander_Corporate")]
    Santander_Corporate,

    [EnumMember(Value = "Starling_Starling")]
    Starling_Starling,

    [EnumMember(Value = "Tsb_Tsb")]
    Tsb_Tsb
}
