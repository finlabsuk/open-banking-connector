// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

public static class BankProfileExtensions
{
    public static BankGroup GetBankGroup(this BankProfileEnum bankProfileEnum) =>
        bankProfileEnum switch
        {
            BankProfileEnum.Barclays_Sandbox => BankGroup.Barclays,
            BankProfileEnum.Barclays_Personal => BankGroup.Barclays,
            BankProfileEnum.Barclays_Wealth => BankGroup.Barclays,
            BankProfileEnum.Barclays_Barclaycard => BankGroup.Barclays,
            BankProfileEnum.Barclays_Business => BankGroup.Barclays,
            BankProfileEnum.Barclays_Corporate => BankGroup.Barclays,
            BankProfileEnum.Barclays_BarclaycardCommercialPayments => BankGroup.Barclays,
            BankProfileEnum.Cooperative_Cooperative => BankGroup.Cooperative,
            BankProfileEnum.Cooperative_CooperativeSandbox => BankGroup.Cooperative,
            BankProfileEnum.Cooperative_Smile => BankGroup.Cooperative,
            BankProfileEnum.Danske_Sandbox => BankGroup.Danske,
            BankProfileEnum.Hsbc_FirstDirect => BankGroup.Hsbc,
            BankProfileEnum.Hsbc_Sandbox => BankGroup.Hsbc,
            BankProfileEnum.Hsbc_UkBusiness => BankGroup.Hsbc,
            BankProfileEnum.Hsbc_UkKinetic => BankGroup.Hsbc,
            BankProfileEnum.Hsbc_UkPersonal => BankGroup.Hsbc,
            BankProfileEnum.Hsbc_HsbcNetUk => BankGroup.Hsbc,
            BankProfileEnum.Lloyds_Sandbox => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_LloydsPersonal => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_LloydsBusiness => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_LloydsCommerical => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_HalifaxPersonal => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_BankOfScotlandPersonal => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_BankOfScotlandBusiness => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_BankOfScotlandCommerical => BankGroup.Lloyds,
            BankProfileEnum.Lloyds_MbnaPersonal => BankGroup.Lloyds,
            BankProfileEnum.Monzo_Monzo => BankGroup.Monzo,
            BankProfileEnum.Monzo_Sandbox => BankGroup.Monzo,
            BankProfileEnum.Nationwide_Nationwide => BankGroup.Nationwide,
            BankProfileEnum.NatWest_NatWestSandbox => BankGroup.NatWest,
            BankProfileEnum.NatWest_NatWest => BankGroup.NatWest,
            BankProfileEnum.NatWest_NatWestBankline => BankGroup.NatWest,
            BankProfileEnum.NatWest_NatWestClearSpend => BankGroup.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotlandSandbox => BankGroup.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotland => BankGroup.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotlandBankline => BankGroup.NatWest,
            BankProfileEnum.NatWest_RoyalBankOfScotlandClearSpend => BankGroup.NatWest,
            BankProfileEnum.NatWest_TheOne => BankGroup.NatWest,
            BankProfileEnum.NatWest_NatWestOne => BankGroup.NatWest,
            BankProfileEnum.NatWest_VirginOne => BankGroup.NatWest,
            BankProfileEnum.NatWest_UlsterBankNi => BankGroup.NatWest,
            BankProfileEnum.NatWest_UlsterBankNiBankline => BankGroup.NatWest,
            BankProfileEnum.NatWest_UlsterBankNiClearSpend => BankGroup.NatWest,
            BankProfileEnum.NatWest_Mettle => BankGroup.NatWest,
            BankProfileEnum.NatWest_Coutts => BankGroup.NatWest,
            BankProfileEnum.Obie_Model2023 => BankGroup.Obie,
            BankProfileEnum.Revolut_Revolut => BankGroup.Revolut,
            BankProfileEnum.Santander_Santander => BankGroup.Santander,
            BankProfileEnum.Santander_Personal => BankGroup.Santander,
            BankProfileEnum.Santander_Business => BankGroup.Santander,
            BankProfileEnum.Santander_Corporate => BankGroup.Santander,
            BankProfileEnum.Starling_Starling => BankGroup.Starling,
            BankProfileEnum.Tsb_Tsb => BankGroup.Tsb,
            _ => throw new ArgumentOutOfRangeException(nameof(bankProfileEnum), bankProfileEnum, null)
        };
}
