// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using ValidationResult = FluentValidation.Results.ValidationResult;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPaymentConsent : Base, ISupportsValidation
    {
        /// <summary>
        ///     Specifies BankRegistration object to use when creating the consent.
        ///     Both PaymentInitiationApiId and BankRegistrationId properties must refer
        ///     to objects with the same parent Bank object.
        /// </summary>
        [Required]
        public Guid BankRegistrationId { get; set; }

        /// <summary>
        ///     Specifies AccountAndTransactionApi object (bank functional API info) to use when creating the consent.
        ///     Both PaymentInitiationApiId and BankRegistrationId properties must refer
        ///     to objects with the same parent Bank object.
        /// </summary>
        [Required]
        public Guid PaymentInitiationApiId { get; set; }

        /// <summary>
        ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
        ///     to translate this for banks supporting an earlier spec version.
        /// </summary>
        [Required]
        public PaymentInitiationModelsPublic.OBWriteDomesticConsent4 ExternalApiRequest { get; set; } = null!;

        public async Task<ValidationResult> ValidateAsync() =>
            await new DomesticPaymentConsentValidator()
                .ValidateAsync(this)!;
    }
}
