// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    /// <summary>
    ///     Banks used for testing.
    /// </summary>
    public enum BankProfileEnum
    {
        Modelo,
        NatWest,
        VrpHackathon,
        Monzo,
        BankOfIreland,
        Nationwide,
        Lloyds
    }

    public static class BankProfileEnumHelper
    {
        static BankProfileEnumHelper()
        {
            AllBanks = Enum.GetValues(typeof(BankProfileEnum)).Cast<BankProfileEnum>();
        }

        public static IEnumerable<BankProfileEnum> AllBanks { get; }

        /// <summary>
        ///     Convert BankEnum to Bank
        /// </summary>
        /// <param name="bankProfileEnum"></param>
        /// <param name="bankProfileDefinitions"></param>
        /// <returns></returns>
        public static BankProfile GetBank(
            BankProfileEnum bankProfileEnum,
            BankProfileDefinitions bankProfileDefinitions) =>
            bankProfileEnum switch
            {
                BankProfileEnum.Modelo => bankProfileDefinitions.Modelo,
                BankProfileEnum.NatWest => bankProfileDefinitions.NatWest,
                BankProfileEnum.VrpHackathon => bankProfileDefinitions.VrpHackathon,
                _ => throw new ArgumentException(
                    $"{nameof(bankProfileEnum)} is not valid ${nameof(BankProfileEnum)} or needs to be added to this switch statement.")
            };
    }
}
