// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation;
using FluentValidation.Results;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    public class DomesticPayment : Base, ISupportsValidation
    {
        /// <summary>
        ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
        ///     to translate this for banks supporting an earlier spec version.
        ///     This request object can also be generated from the Open Banking consent request object via a type mapping.
        ///     The value of "Data.ConsentId" should be consistent with the external API ID (bank ID) for the supplied
        ///     DomesticPaymentConsent or simply
        ///     left set to null in which case the correct value will be substituted.
        /// </summary>
        public PaymentInitiationModelsPublic.OBWriteDomestic2 ExternalApiRequest { get; set; } = null!;

        public async Task<ValidationResult> ValidateAsync() =>
            await new DomesticPaymentValidator()
                .ValidateAsync(this)!;
    }
}
