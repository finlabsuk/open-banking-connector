// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

/// <summary>
///     Values included in auth URL by Co-operative bank group.
///     From: https://www.developer.co-operativebank.co.uk/apis/general-specifications/
/// </summary>
public class DomesticPaymentConsentAuthContextCooperativeOnly
{
    /// <summary>
    ///     Creditor account name (Data.Initiation.CreditorAccount.Name) as sent in the consent request.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string CreditorAccountName { get; init; }

    /// <summary>
    ///     Instructed amount (Data.Initiation.InstructedAmount.Amount) as sent in the consent request.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string InstructedAmountAmount { get; init; }
}

public class DomesticPaymentConsentAuthContext : AuthContextBase, ISupportsValidation
{
    [JsonProperty(Required = Required.Always)]
    public required Guid DomesticPaymentConsentId { get; init; }

    /// <summary>
    ///     Required for (and only used by) Co-operative bank group, otherwise ignored.
    /// </summary>
    public DomesticPaymentConsentAuthContextCooperativeOnly? CooperativeOnly { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticPaymentConsentAuthContextValidator()
            .ValidateAsync(this);
}
