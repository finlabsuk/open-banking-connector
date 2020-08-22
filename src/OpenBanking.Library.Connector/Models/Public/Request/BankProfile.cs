// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    public class BankProfile
    {
        /// <summary>
        ///     Specifies bank registration this profile will be based on.
        ///     The registration is here specified directly by its ID.
        ///     Use this or <see cref="BankName" /> to specify the bank registration but not both!
        /// </summary>
        public string? BankRegistrationId { get; set; }

        /// <summary>
        ///     Specifies bank registration this profile will be based on.
        ///     The registration is here specified indirectly by a bank name. The bank name is used
        ///     to perform a lookup of <see cref="Persistent.Bank.DefaultBankRegistrationId" /> or
        ///     <see cref="Persistent.Bank.StagingBankRegistrationId" /> (depending on
        ///     <see cref="UseStagingBankRegistration" />)
        ///     to get the bank registration.
        ///     Use this or <see cref="BankRegistrationId" /> to specify the bank registration but not both!
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        ///     Used when <see cref="BankName" /> is not null to select between <see cref="Persistent.Bank.DefaultBankProfileId" />
        ///     and
        ///     <see cref="Persistent.Bank.StagingBankProfileId" />.
        /// </summary>
        public bool UseStagingBankRegistration { get; set; } = false;

        /// <summary>
        ///     If profile is successfully created, replace <see cref="Persistent.Bank.DefaultBankProfileId" />
        ///     with reference to this profile.
        /// </summary>
        public bool ReplaceDefaultBankProfile { get; set; } = false;

        /// <summary>
        ///     If profile is successfully created, replace <see cref="Persistent.Bank.StagingBankProfileId" />
        ///     with reference to this profile.
        /// </summary>
        public bool ReplaceStagingBankProfile { get; set; } = false;

        /// <summary>
        ///     Specifies UK Open Banking Payment Initiation API associated with profile.
        ///     Null means profile is not used with such an API.
        /// </summary>
        public PaymentInitiationApi? PaymentInitiationApi { get; set; }
    }
}
