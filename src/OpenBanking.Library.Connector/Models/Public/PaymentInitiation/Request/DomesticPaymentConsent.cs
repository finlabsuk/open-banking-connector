// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation;
using FluentValidation.Results;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPaymentConsent : Base, ISupportsValidation
    {
        /// <summary>
        ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
        ///     to translate this for banks supporting an earlier spec version.
        /// </summary>
        public PaymentInitiationModelsPublic.OBWriteDomesticConsent4 OBWriteDomesticConsent { get; set; } = null!;

        /// <summary>
        ///     Specifies Bank API Set to use when creating consent.
        ///     Both <see cref="BankApiSetId" /> and <see cref="BankRegistrationId" /> must point to objects with the same parent
        ///     (Bank ID).
        /// </summary>
        public Guid BankApiSetId { get; set; }

        /// <summary>
        ///     Specifies Bank Registration to use when creating consent.
        ///     Both <see cref="BankApiSetId" /> and <see cref="BankRegistrationId" /> must point to objects with the same parent
        ///     (Bank ID).
        /// </summary>
        public Guid BankRegistrationId { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new DomesticPaymentConsentValidator()
                .ValidateAsync(this)!;
    }
}
