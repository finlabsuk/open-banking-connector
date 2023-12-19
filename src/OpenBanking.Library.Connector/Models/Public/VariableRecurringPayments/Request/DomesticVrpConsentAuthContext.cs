// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Validators;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

public class DomesticVrpConsentAuthContext : EntityBase, ISupportsValidation
{
    [JsonProperty(Required = Required.Always)]
    public required Guid DomesticVrpConsentId { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticVrpConsentAuthContextValidator()
            .ValidateAsync(this);
}
