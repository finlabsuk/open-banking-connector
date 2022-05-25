// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public class BankProfileHiddenPropertiesDictionary : Dictionary<BankProfileEnum, BankProfileHiddenProperties> { }

    public partial class BankProfileDefinitions
    {
        private readonly BankProfileHiddenPropertiesDictionary _hiddenPropertiesDictionary;
        private readonly Lazy<BankProfile> _hsbcPersonal;
        private readonly Lazy<BankProfile> _hsbcSandbox;

        public BankProfileDefinitions(BankProfileHiddenPropertiesDictionary bankProfileHiddenPropertiesDataProvider)
        {
            // Store bank profile hidden properties
            _hiddenPropertiesDictionary = bankProfileHiddenPropertiesDataProvider;

            // Initialise bank profiles
            Modelo = GetModelo();
            NatWest = GetNatWest();
            VrpHackathon = GetVrpHackathon();
            Santander = GetSantander();
            Barclays = GetBarclays();
            RoyalBankOfScotland = GetRoyalBankOfScotland();
            NewDayAmazon = GetNewDayAmazon();
            Nationwide = GetNationwide();
            _hsbcSandbox =
                new Lazy<BankProfile>(
                    () =>
                        new Hsbc().GetBankProfile(HsbcBank.Sandbox, _hiddenPropertiesDictionary));
            _hsbcPersonal =
                new Lazy<BankProfile>(
                    () =>
                        new Hsbc().GetBankProfile(HsbcBank.UkPersonal, _hiddenPropertiesDictionary));
            Danske = GetDanske();
            AlliedIrish = GetAlliedIrish();
            Monzo = GetMonzo();
            Lloyds = GetLloyds();
            Tsb = GetTsb();
        }

        public BankProfile GetBankProfile(BankProfileEnum bankProfileEnum) =>
            bankProfileEnum switch
            {
                BankProfileEnum.Modelo => Modelo,
                BankProfileEnum.NatWest => NatWest,
                BankProfileEnum.VrpHackathon => VrpHackathon,
                BankProfileEnum.Santander => Santander,
                BankProfileEnum.Barclays => Barclays,
                BankProfileEnum.RoyalBankOfScotland => RoyalBankOfScotland,
                BankProfileEnum.NewDayAmazon => NewDayAmazon,
                BankProfileEnum.Nationwide => Nationwide,
                BankProfileEnum.Lloyds => Lloyds,
                BankProfileEnum.Hsbc_Sandbox => _hsbcSandbox.Value,
                BankProfileEnum.Hsbc_UkPersonal => _hsbcPersonal.Value,
                BankProfileEnum.Danske => Danske,
                BankProfileEnum.AlliedIrish => AlliedIrish,
                BankProfileEnum.Monzo => Monzo,
                BankProfileEnum.Tsb => Tsb,
                _ => throw new ArgumentOutOfRangeException(nameof(bankProfileEnum), bankProfileEnum, null)
            };


        private BankProfileHiddenProperties GetRequiredBankProfileHiddenProperties(BankProfileEnum bankProfileEnum)
        {
            if (!_hiddenPropertiesDictionary.TryGetValue(
                    bankProfileEnum,
                    out BankProfileHiddenProperties? bankProfileHiddenProperties))
            {
                throw new ArgumentNullException(
                    $"Hidden properties are required for bank profile {bankProfileEnum} " +
                    "and cannot be found.");
            }

            return bankProfileHiddenProperties;
        }
    }
}
