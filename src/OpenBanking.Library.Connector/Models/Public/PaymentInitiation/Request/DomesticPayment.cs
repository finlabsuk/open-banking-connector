// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.PaymentInitialisation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DomesticPaymentTemplateType
    {
        [EnumMember(Value = "PersonToPersonExample")]
        PersonToPersonExample,

        [EnumMember(Value = "PersonToMerchantExample")]
        PersonToMerchantExample
    }

    public class DomesticPaymentTemplateParameters
    {
        public string InstructionIdentification { get; set; } = null!;
        public string EndToEndIdentification { get; set; } = null!;
    }

    public class DomesticPaymentTemplateRequest
    {
        /// <summary>
        ///     Template type to use.
        /// </summary>
        public DomesticPaymentTemplateType Type { get; set; }

        /// <summary>
        ///     Template parameters.
        /// </summary>
        public DomesticPaymentTemplateParameters Parameters { get; set; } = null!;
    }

    public class DomesticPaymentRequest : Base, ISupportsValidation
    {
        /// <summary>
        ///     Use external API request object created from template.
        ///     The first non-null of ExternalApiRequest and TemplateRequest (in that order) is used
        ///     and the others are ignored. At least one of these must be non-null.
        ///     Specifies template used to create external API request object.
        /// </summary>
        public DomesticPaymentTemplateRequest? TemplateRequest { get; set; }

        /// <summary>
        ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
        ///     to translate this for banks supporting an earlier spec version.
        ///     This request object can also be generated from the Open Banking consent request object via a type mapping.
        ///     The value of "Data.ConsentId" should be consistent with the external API ID (bank ID) for the supplied
        ///     DomesticPaymentConsent or simply
        ///     left set to null in which case the correct value will be substituted.
        /// </summary>
        public PaymentInitiationModelsPublic.OBWriteDomestic2? ExternalApiRequest { get; set; }

        public async Task<ValidationResult> ValidateAsync() =>
            await new DomesticPaymentValidator()
                .ValidateAsync(this)!;
    }
}
