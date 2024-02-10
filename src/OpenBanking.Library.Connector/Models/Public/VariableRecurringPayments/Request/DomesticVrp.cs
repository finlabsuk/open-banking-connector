// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

public class DomesticVrpRequest : ISupportsValidation
{
    /// <summary>
    ///     Specifies Domestic VRP Consent to use when creating payment.
    ///     If domestic VRP consent has been successfully authorised, a token will be associated with the consent which can
    ///     be used to create a VRP payment.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required Guid DomesticVrpConsentId { get; set; }

    /// <summary>
    ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
    ///     to translate this for banks supporting an earlier spec version.
    ///     This request object can also be generated from the Open Banking consent request object via a type mapping.
    ///     The value of "Data.ConsentId" should be consistent with the external API ID (bank ID) for the supplied
    ///     DomesticVrpConsent or simply
    ///     left set to null in which case the correct value will be substituted.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest ExternalApiRequest { get; init; }

    public string? ModifiedBy { get; set; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticVrpValidator()
            .ValidateAsync(this);
}
