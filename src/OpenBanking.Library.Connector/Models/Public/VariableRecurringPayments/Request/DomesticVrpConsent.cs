// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Validators;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

public class DomesticVrpConsentRequest : ConsentBase, ISupportsValidation
{
    /// <summary>
    ///     Request object OBDomesticVRPConsentRequest from UK Open Banking Read-Write Variable Recurring Payments API spec.
    /// </summary>
    public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest? ExternalApiRequest { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticVrpConsentValidator()
            .ValidateAsync(this);
}
