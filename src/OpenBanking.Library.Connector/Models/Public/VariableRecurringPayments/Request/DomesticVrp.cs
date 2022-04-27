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
    public class DomesticVrp : Base, ISupportsValidation
    {
        /// <summary>
        ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
        ///     to translate this for banks supporting an earlier spec version.
        ///     This request object can also be generated from the Open Banking consent request object via a type mapping.
        ///     The value of "Data.ConsentId" should be consistent with the external API ID (bank ID) for the supplied
        ///     DomesticVrpConsent or simply
        ///     left set to null in which case the correct value will be substituted.
        /// </summary>
        [Required]
        [JsonProperty(Required = Required.Always)]
        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest ExternalApiRequest { get; set; } = null!;

        public async Task<ValidationResult> ValidateAsync() =>
            await new DomesticVrpValidator()
                .ValidateAsync(this)!;
    }
}
