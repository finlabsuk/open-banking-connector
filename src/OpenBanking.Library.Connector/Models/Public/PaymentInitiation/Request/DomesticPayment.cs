// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

public class DomesticPaymentRequest : ISupportsValidation
{
    /// <summary>
    ///     Specifies Domestic Payment Consent to use when creating payment.
    ///     If domestic payment consent has been successfully authorised, a token will be associated with the consent which can
    ///     be used to create the payment.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required Guid DomesticPaymentConsentId { get; set; }

    /// <summary>
    ///     Request object OBWriteDomestic2 from UK Open Banking Read-Write Payment Initiation API spec.
    ///     This request object can also be generated from the Open Banking consent request object via a type mapping.
    ///     The value of "Data.ConsentId" should be consistent with the external API ID (bank ID) for the supplied
    ///     DomesticPaymentConsent or simply
    ///     left set to null in which case the correct value will be substituted.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required PaymentInitiationModelsPublic.OBWriteDomestic2 ExternalApiRequest { get; init; }

    public string? ModifiedBy { get; set; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticPaymentValidator()
            .ValidateAsync(this);
}
