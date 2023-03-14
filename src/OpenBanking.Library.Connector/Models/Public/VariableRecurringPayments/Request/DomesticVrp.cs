// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.VariableRecurringPayments;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

[JsonConverter(typeof(StringEnumConverter))]
public enum DomesticVrpTemplateType
{
    [EnumMember(Value = "VrpWithDebtorAccountSpecifiedByPisp")]
    VrpWithDebtorAccountSpecifiedByPisp, // OB example (https://openbankinguk.github.io/read-write-api-site3/v3.1.9/references/usage-examples/vrp-usage-examples.html)

    [EnumMember(Value = "VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation")]
    VrpWithDebtorAccountSpecifiedDuringConsentAuthorisation, // same as VrpWithDebtorAccountSpecifiedByPisp but no debtor account specified

    [EnumMember(
        Value =
            "VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation")]
    VrpWithDebtorAccountSpecifiedDuringConsentAuthorisationAndCreditorAccountSpecifiedDuringPaymentInitiation // OB example (https://openbankinguk.github.io/read-write-api-site3/v3.1.9/references/usage-examples/vrp-usage-examples.html)
}

public class DomesticVrpTemplateParameters
{
    public string InstructionIdentification { get; set; } = null!;
    public string EndToEndIdentification { get; set; } = null!;
}

public class DomesticVrpTemplateRequest
{
    /// <summary>
    ///     Template type to use.
    /// </summary>
    public DomesticVrpTemplateType Type { get; set; }

    public DomesticVrpTemplateParameters Parameters { get; set; } = null!;
}

public class DomesticVrpRequest : Base, ISupportsValidation
{
    /// <summary>
    ///     BankProfile used to apply transformations to external API requests.
    /// </summary>
    public BankProfileEnum? BankProfile { get; set; }

    /// <summary>
    ///     Use external API request object created from template.
    ///     The first non-null of ExternalApiRequest and TemplateRequest (in that order) is used
    ///     and the others are ignored. At least one of these must be non-null.
    ///     Specifies template used to create external API request object.
    /// </summary>
    public DomesticVrpTemplateRequest? TemplateRequest { get; set; }

    /// <summary>
    ///     Request object from recent version of UK Open Banking spec. Open Banking Connector can be configured
    ///     to translate this for banks supporting an earlier spec version.
    ///     This request object can also be generated from the Open Banking consent request object via a type mapping.
    ///     The value of "Data.ConsentId" should be consistent with the external API ID (bank ID) for the supplied
    ///     DomesticVrpConsent or simply
    ///     left set to null in which case the correct value will be substituted.
    /// </summary>
    public VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest? ExternalApiRequest { get; set; } = null!;

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticVrpValidator()
            .ValidateAsync(this)!;
}
