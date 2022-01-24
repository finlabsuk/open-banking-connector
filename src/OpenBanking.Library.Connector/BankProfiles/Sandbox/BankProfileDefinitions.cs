// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        private readonly Dictionary<string, Dictionary<string, BankProfileHiddenProperties>>
            _bankProfileHiddenProperties;

        public BankProfileDefinitions(
            Dictionary<string, Dictionary<string, BankProfileHiddenProperties>> bankProfileHiddenPropertiesDataProvider)
        {
            // Store bank profile hidden properties
            _bankProfileHiddenProperties = bankProfileHiddenPropertiesDataProvider;

            // Initialise bank profiles
            Modelo = GetModelo();
            NatWest = GetNatWest();
            VrpHackathon = GetVrpHackathon();
            Santander = GetSantander();
            Barclays = GetBarclays();
            RoyalBankOfScotland = GetRoyalBankOfScotland();
            NewDayAmazon = GetNewDayAmazon();
            Nationwide = GetNationwide();
            Hsbc = GetHsbc();
            Danske = GetDanske();
            AlliedIrish = GetAlliedIrish();
            Monzo = GetMonzo();
            Lloyds = GetLloyds();
            TSB = GetTSB();
        }

        private BankProfileHiddenProperties GetRequiredBankProfileHiddenProperties(BankProfileEnum bankProfileEnum)
        {
            // Get "Sandbox" dictionary
            Dictionary<string, BankProfileHiddenProperties> innerDict =
                _bankProfileHiddenProperties["Sandbox"] ?? throw new Exception("No Sandbox bank profiles found.");

            // Get bankProfileEnum dictionary
            BankProfileHiddenProperties bankProfileHiddenProperties =
                innerDict[bankProfileEnum.ToString()] ??
                throw new Exception($"No bank profile found for {bankProfileEnum}.");

            return bankProfileHiddenProperties;
        }
    }
}
