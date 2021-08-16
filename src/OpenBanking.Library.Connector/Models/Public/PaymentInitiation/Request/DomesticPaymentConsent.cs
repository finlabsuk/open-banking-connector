// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation;
using FluentValidation.Results;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p6.PaymentInitiation.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPaymentConsent : Base, ISupportsValidation
    {
        public PaymentInitiationModelsPublic.OBWriteDomesticConsent4 WriteDomesticConsent { get; set; } = null!;

        /// <summary>
        ///     Specifies BankProfile to be used for consent.
        ///     When not specified (null), <see cref="BankId" /> must be specified (non-null) and will be used to
        ///     specify the BankProfile according to <see cref="Persistent.Bank.DefaultBankProfileId" /> or
        ///     <see cref="Persistent.Bank.StagingBankProfileId" /> (depending on
        ///     <see cref="UseStagingNotDefaultBankProfile" />).
        /// </summary>
        public Guid BankApiInformationId { get; set; }

        /// <summary>
        ///     Specifies BankRegistration to be used for consent.
        ///     When not specified (null), <see cref="BankId" /> must be specified (non-null) and will be used to
        ///     specify the BankRegistration according to <see cref="Persistent.Bank.DefaultBankRegistrationId" /> or
        ///     <see cref="Persistent.Bank.StagingBankRegistrationId" /> (depending on
        ///     <see cref="UseStagingNotDefaultBankRegistration" />).
        /// </summary>
        public Guid BankRegistrationId { get; set; }


        public async Task<ValidationResult> ValidateAsync() =>
            await new DomesticPaymentConsentValidator()
                .ValidateAsync(this)!;
    }
}
