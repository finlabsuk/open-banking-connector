// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPaymentConsent
    {
        public OBWriteDomesticConsent4 DomesticConsent { get; set; } = null!;

        /// <summary>
        ///     Specifies bank profile to be used for consent.
        ///     The profile is here specified directly by its ID.
        ///     Use this or <see cref="BankName" /> to specify the bank profile but not both!
        /// </summary>
        public string? BankProfileId { get; set; }

        /// <summary>
        ///     Specifies bank profile to be used for consent.
        ///     The profile is here specified indirectly by a bank name. The bank name is used
        ///     to perform a lookup of <see cref="Persistent.Bank.DefaultBankProfileId" /> or
        ///     <see cref="Persistent.Bank.StagingBankProfileId" /> (depending on
        ///     <see cref="UseStagingBankProfile" />)
        ///     to get the bank profile.
        ///     Use this or <see cref="BankProfileId" /> to specify the bank profile but not both!
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        ///     Used when <see cref="BankName" /> is not null to select between <see cref="Persistent.Bank.DefaultBankProfileId" />
        ///     and
        ///     <see cref="Persistent.Bank.StagingBankProfileId" />.
        /// </summary>
        public bool UseStagingBankProfile { get; set; }
    }
}
