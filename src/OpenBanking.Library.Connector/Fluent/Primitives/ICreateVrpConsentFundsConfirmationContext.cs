// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for creating a VRP consent funds confirmation
/// </summary>
public interface ICreateVrpConsentFundsConfirmationContext
{
    /// <summary>
    ///     Create funds confirmation for VRP consent at external (bank) API
    /// </summary>
    /// <param name="createParams"></param>
    /// <returns></returns>
    Task<DomesticVrpConsentFundsConfirmationResponse> CreateFundsConfirmationAsync(
        VrpConsentFundsConfirmationCreateParams createParams);
}
