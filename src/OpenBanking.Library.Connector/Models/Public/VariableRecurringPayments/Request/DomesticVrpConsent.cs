// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.VariableRecurringPayments;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request
{
    public class DomesticVrpConsent : Base, ISupportsValidation
    {
        /// <summary>
        ///     Specifies BankRegistration object to use when creating the consent.
        ///     Both VariableRecurringPaymentsApiId and BankRegistrationId properties must refer
        ///     to objects with the same parent Bank object.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public Guid BankRegistrationId { get; set; }

        /// <summary>
        ///     Specifies AccountAndTransactionApi object (bank functional API info) to use when creating the consent.
        ///     Both VariableRecurringPaymentsApiId and BankRegistrationId properties must refer
        ///     to objects with the same parent Bank object.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public Guid VariableRecurringPaymentsApiId { get; set; }

        /// <summary>
        ///     Request object from recent version of UK Open Banking spec. Where applicable, Open Banking Connector can be
        ///     configured
        ///     to translate this for banks supporting an earlier spec version.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest ExternalApiRequest { get; set; } =
            null!;

        public async Task<ValidationResult> ValidateAsync() =>
            await new DomesticVrpConsentValidator()
                .ValidateAsync(this)!;
    }
}
