// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;

[JsonConverter(typeof(StringEnumConverter))]
public enum AccountAccessConsentTemplateType
{
    [EnumMember(Value = "MaximumPermissions")]
    MaximumPermissions
}

public class AccountAccessConsentTemplateParameters { }

public class AccountAccessConsentTemplateRequest
{
    /// <summary>
    ///     Template type to use.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required AccountAccessConsentTemplateType Type { get; init; }

    /// <summary>
    ///     Template parameters.
    /// </summary>
    public AccountAccessConsentTemplateParameters? Parameters { get; init; }
}

/// <summary>
///     Request object used to create an AccountAccessConsent. Includes a UK Open Banking request object
///     plus information on the bank registration and bank functional API to use for the consent.
/// </summary>
public class AccountAccessConsentRequest : ConsentBase, ISupportsValidation
{
    /// <summary>
    ///     Use external API request object created from template.
    ///     The first non-null of ExternalApiObject, ExternalApiRequest, and TemplateRequest (in that order) is used
    ///     and the others are ignored. At least one of these three must be non-null.
    ///     Specifies template used to create external API request object.
    /// </summary>
    public AccountAccessConsentTemplateRequest? TemplateRequest { get; init; }

    /// <summary>
    ///     Use directly-specified external API request object.
    ///     The first non-null of ExternalApiObject, ExternalApiRequest, and TemplateRequest (in that order) is used
    ///     and the others are ignored. At least one of these three must be non-null.
    ///     Specifies OBReadConsent1 from UK Open Banking Read-Write Account and Transaction API spec
    ///     <a
    ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.8r5/dist/openapi/account-info-openapi.yaml" />
    ///     v3.1.9r5 <a />. Open Banking Connector will automatically
    ///     translate <i>from</i> this to an older format for banks supporting an earlier spec version.
    /// </summary>
    public AccountAndTransactionModelsPublic.OBReadConsent1? ExternalApiRequest { get; set; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new AccountAccessConsentValidator()
            .ValidateAsync(this);
}
