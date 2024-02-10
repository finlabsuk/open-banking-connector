﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Validators;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;

public class DomesticVrpConsentRequest : ConsentBase, ISupportsValidation
{
    /// <summary>
    ///     Request object from recent version of UK Open Banking spec. Where applicable, Open Banking Connector can be
    ///     configured
    ///     to translate this for banks supporting an earlier spec version.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest ExternalApiRequest { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new DomesticVrpConsentValidator()
            .ValidateAsync(this);
}
