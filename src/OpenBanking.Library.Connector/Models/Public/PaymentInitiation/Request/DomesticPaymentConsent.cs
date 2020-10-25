// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPaymentConsent
    {
        public OBRisk1 Merchant { get; set; } = null!;

        public string? LocalInstrument { get; set; }

        public OBWriteDomestic2DataInitiationCreditorAccount CreditorAccount { get; set; } = null!;

        public OBWriteDomestic2DataInitiationDebtorAccount? DebtorAccount { get; set; }

        public OBWriteDomestic2DataInitiationInstructedAmount InstructedAmount { get; set; } = null!;

        public string InstructionIdentification { get; set; } = null!;

        public string EndToEndIdentification { get; set; } = null!;

        public OBWriteDomesticConsent4DataAuthorisation? Authorisation { get; set; }

        public OBWriteDomestic2DataInitiationRemittanceInformation RemittanceInformation { get; set; } = null!;

        /// <summary>
        ///     Specifies BankProfile to be used for consent.
        ///     When not specified (null), <see cref="BankId" /> must be specified (non-null) and will be used to
        ///     specify the BankProfile according to <see cref="Persistent.Bank.DefaultBankProfileId" /> or
        ///     <see cref="Persistent.Bank.StagingBankProfileId" /> (depending on
        ///     <see cref="UseStagingNotDefaultBankProfile" />).
        /// </summary>
        public Guid? BankProfileId { get; set; }

        /// <summary>
        ///     Specifies BankRegistration to be used for consent.
        ///     When not specified (null), <see cref="BankId" /> must be specified (non-null) and will be used to
        ///     specify the BankRegistration according to <see cref="Persistent.Bank.DefaultBankRegistrationId" /> or
        ///     <see cref="Persistent.Bank.StagingBankRegistrationId" /> (depending on
        ///     <see cref="UseStagingNotDefaultBankRegistration" />).
        /// </summary>
        public Guid? BankRegistrationId { get; set; }

        /// <summary>
        ///     Used when <see cref="BankId" /> specifies BankProfile to select between
        ///     <see cref="Persistent.Bank.DefaultBankProfileId" />
        ///     and
        ///     <see cref="Persistent.Bank.StagingBankProfileId" />.
        ///     See <see cref="BankProfileId" /> for info on how BankProfile is specified.
        /// </summary>
        public bool UseStagingNotDefaultBankProfile { get; set; }

        /// <summary>
        ///     Used when <see cref="BankId" /> specifies BankRegistration to select between
        ///     <see cref="Persistent.Bank.DefaultBankRegistrationId" />
        ///     and
        ///     <see cref="Persistent.Bank.StagingBankRegistrationId" />.
        ///     See <see cref="BankRegistrationId" /> for info on how BankRegistration is specified.
        /// </summary>
        public bool UseStagingNotDefaultBankRegistration { get; set; } = false;

        /// <summary>
        ///     Used to specify BankRegistration for consent when <see cref="BankRegistrationId" /> is null.
        ///     Used to specify BankProfile for consent when <see cref="BankProfileId" /> is null.
        ///     Otherwise ignored.
        /// </summary>
        public Guid? BankId { get; set; }
    }
}
