// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Validators.AccountAndTransaction;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;

public class AccountAccessConsentAuthContext : Base, ISupportsValidation
{
    [Required]
    [JsonProperty(Required = Required.Always)]
    public Guid AccountAccessConsentId { get; set; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new AccountAccessConsentAuthContextValidator()
            .ValidateAsync(this)!;
}
